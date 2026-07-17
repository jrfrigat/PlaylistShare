using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlaylistShare.Api.Extensions;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared;
using PlaylistShare.Shared.SharedPlaylist;
using PlaylistShare.Shared.Yandex;

namespace PlaylistShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SharedPlaylistController : ControllerBase
{
    private readonly SharedPlaylistService _sharedService;
    private readonly YandexMusicService _yandexService;
    private readonly UserSessionService _userSessionService;
    private readonly TrackAdditionLogService _trackAdditionLogService;
    private readonly TrackRemovalLogService _trackRemovalLogService;

    public SharedPlaylistController(
        SharedPlaylistService sharedService,
        YandexMusicService yandexService,
        TrackAdditionLogService trackAdditionLogService,
        TrackRemovalLogService trackRemovalLogService,
        UserSessionService userSessionService)
    {
        _sharedService = sharedService;
        _yandexService = yandexService;
        _trackAdditionLogService = trackAdditionLogService;
        _trackRemovalLogService = trackRemovalLogService;
        _userSessionService = userSessionService;
    }

    // GET /api/sharedplaylist/{token}
    [HttpGet("{token}")]
    public async Task<ActionResult<ApiResponse<SharedPlaylistDto>>> GetByToken(string token, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserIdOrNull();

        var entity = await _sharedService.GetEntityByTokenAsync(token, cancellationToken);
        if (entity == null)
            return NotFound(ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (!_sharedService.CanView(entity, currentUserId))
            return Unauthorized(ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { StatusCode = 401, Message = "Недостаточно прав" }));

        return Ok(ApiResponse<SharedPlaylistDto>.Ok(_sharedService.MapToDto(entity)));
    }

    // GET /api/sharedplaylist/{token}/tracks
    [HttpGet("{token}/tracks")]
    public async Task<ActionResult<ApiResponse<YandexPlaylistData>>> GetTracks(string token, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserIdOrNull();
        var playlist = await _sharedService.GetEntityByTokenAsync(token, cancellationToken);
        if (playlist == null)
            return NotFound(ApiResponse<YandexPlaylistData>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (!_sharedService.CanView(playlist, currentUserId))
            return Unauthorized();

        var creator = playlist.Creator;
        if (creator == null)
            return StatusCode(500, ApiResponse<YandexPlaylistData>.Fail(new ErrorResponse { StatusCode = 500, Message = "Владелец плейлиста не найден" }));

        var dto = await _yandexService.GetPlaylistDataAsync(creator, playlist.YandexPlaylistOwnerUid, playlist.YandexPlaylistKind, cancellationToken);
        if (dto == null)
            return NotFound(ApiResponse<YandexPlaylistData>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден в Яндекс.Музыке" }));

        dto.RemovableTrackIds = await _sharedService.GetRemovableTrackIdsAsync(
            playlist,
            currentUserId,
            dto.Tracks.Select(t => t.TrackId).ToList(),
            async ct => (await _userSessionService.GetOrCreateCurrentSessionAsync(currentUserId, ct)).SessionId,
            cancellationToken);

        return Ok(ApiResponse<YandexPlaylistData>.Ok(dto));
    }

    // PUT /api/sharedplaylist/{token}/permissions
    [HttpPut("{token}/permissions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SharedPlaylistDto>>> UpdatePermissions(string token, [FromBody] UpdatePermissionsDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var playlist = await _sharedService.GetEntityByTokenAsync(token, cancellationToken);
        if (playlist == null)
            return NotFound(ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (playlist.CreatorUserId != userId)
            return Forbid();

        var updated = await _sharedService.UpdatePermissionsAsync(playlist.Id, dto, cancellationToken);
        if (updated == null)
            return BadRequest(ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { StatusCode = 400, Message = "Ошибка обновления прав" }));

        return Ok(ApiResponse<SharedPlaylistDto>.Ok(updated));
    }

    // POST /api/sharedplaylist/{token}/add-tracks
    [HttpPost("{token}/add-tracks")]
    public async Task<ActionResult<ApiResponse<object>>> AddTracks(string token, [FromBody] UpdateTrackListRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserIdOrNull();
        var playlist = await _sharedService.GetEntityByTokenAsync(token, cancellationToken);
        if (playlist == null)
            return NotFound(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        // Просмотр - предусловие правки: право менять список сильнее права его видеть, поэтому и
        // проверяться должно не слабее. Без этого при ViewPermission=AuthorizedOnly и
        // AddPermission=Everyone аноним со ссылкой правил бы плейлист, который ему не разрешено
        // даже открыть.
        if (!_sharedService.CanView(playlist, currentUserId))
            return Unauthorized(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 401, Message = "Недостаточно прав" }));

        if (!_sharedService.CanAddTrack(playlist, currentUserId))
            return StatusCode(403, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 403, Message = "Недостаточно прав для добавления треков" }));

        var creator = playlist.Creator;
        if (creator == null)
            return StatusCode(500, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 500, Message = "Владелец плейлиста не найден" }));

        var updatedPlaylist = await _yandexService.AddTracksAsync(creator, playlist.YandexPlaylistOwnerUid, playlist.YandexPlaylistKind, request.TrackIds, cancellationToken);
        if (updatedPlaylist == null)
            return StatusCode(500, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 500, Message = "Ошибка при добавлении треков" }));

        // Треки уже лежат в плейлисте Яндекса - отменить это мы не можем, поэтому журнал дописываем
        // без токена отмены. Иначе ушедший клиент оставил бы треки без записей о том, кто их добавил,
        // а по ним решается, кому можно их удалять (RemovePermission.AddedByUserOnly).
        var session = await _userSessionService.GetOrCreateCurrentSessionAsync(currentUserId, CancellationToken.None);
        var sessionId = session.SessionId;
        foreach (var trackId in request.TrackIds)
        {
            await _trackAdditionLogService.LogAdditionAsync(playlist.Id, trackId, currentUserId, sessionId, CancellationToken.None);
        }

        return Ok(ApiResponse<object>.Ok(new { message = "Треки добавлены" }));
    }

    // GET /api/sharedplaylist/{token}/additions
    [HttpGet("{token}/additions")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, string?>>>> GetAdditions(string token, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserIdOrNull();
        var playlist = await _sharedService.GetEntityByTokenAsync(token, cancellationToken);
        if (playlist == null)
            return NotFound(ApiResponse<Dictionary<string, string?>>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (!_sharedService.CanView(playlist, currentUserId))
            return Unauthorized();

        var additions = await _trackAdditionLogService.GetAdditionUserNamesAsync(playlist.Id, cancellationToken);
        return Ok(ApiResponse<Dictionary<string, string?>>.Ok(additions));
    }

    // POST /api/sharedplaylist/{token}/remove-tracks
    [HttpPost("{token}/remove-tracks")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveTracks(string token, [FromBody] UpdateTrackListRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserIdOrNull();
        var playlist = await _sharedService.GetEntityByTokenAsync(token, cancellationToken);
        if (playlist == null)
            return NotFound(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        // Просмотр - предусловие правки (см. AddTracks). Проверяем до создания сессии: незачем
        // заводить строку в БД тому, кому и смотреть-то нельзя.
        if (!_sharedService.CanView(playlist, currentUserId))
            return Unauthorized(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 401, Message = "Недостаточно прав" }));

        var session = await _userSessionService.GetOrCreateCurrentSessionAsync(currentUserId, cancellationToken);
        var sessionId = session.SessionId;

        foreach (var trackId in request.TrackIds)
        {
            if (!await _sharedService.CanRemoveTrackAsync(playlist, currentUserId, trackId, sessionId, cancellationToken))
                return StatusCode(403, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 403, Message = $"Недостаточно прав для удаления трека {trackId}" }));
        }

        var creator = playlist.Creator;
        if (creator == null)
            return StatusCode(500, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 500, Message = "Владелец плейлиста не найден" }));

        var updatedPlaylist = await _yandexService.RemoveTracksAsync(creator, playlist.YandexPlaylistOwnerUid, playlist.YandexPlaylistKind, request.TrackIds, cancellationToken);
        if (updatedPlaylist == null)
            return StatusCode(500, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 500, Message = "Ошибка при удалении треков" }));

        // Как и в AddTracks: треков в Яндексе уже нет, журнал приводим в соответствие несмотря на уход клиента.
        foreach (var trackId in request.TrackIds)
        {
            await _trackRemovalLogService.LogRemovalAsync(playlist.Id, trackId, currentUserId, sessionId, CancellationToken.None);
            await _trackAdditionLogService.RemoveLogsForTrackAsync(playlist.Id, trackId, CancellationToken.None);
        }

        return Ok(ApiResponse<object>.Ok(new { message = "Треки удалены" }));
    }

}