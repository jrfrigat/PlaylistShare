using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlaylistShare.Api.Entities;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UserSessionService _userSessionService;
    private readonly TrackAdditionLogService _trackAdditionLogService;
    private readonly TrackRemovalLogService _trackRemovalLogService;

    public SharedPlaylistController(
        SharedPlaylistService sharedService,
        YandexMusicService yandexService,
        UserManager<ApplicationUser> userManager,
        TrackAdditionLogService trackAdditionLogService,
        TrackRemovalLogService trackRemovalLogService,
        UserSessionService userSessionService)
    {
        _sharedService = sharedService;
        _yandexService = yandexService;
        _userManager = userManager;
        _trackAdditionLogService = trackAdditionLogService;
        _trackRemovalLogService = trackRemovalLogService;
        _userSessionService = userSessionService;
    }

    // GET /api/sharedplaylist/{token}
    [HttpGet("{token}")]
    public async Task<ActionResult<ApiResponse<SharedPlaylistDto>>> GetByToken(string token)
    {
        var currentUserId = User.GetUserIdOrNull();

        var entity = await _sharedService.GetEntityByTokenAsync(token);
        if (entity == null)
            return NotFound(ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (!_sharedService.CanView(entity, currentUserId))
            return Unauthorized(ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { StatusCode = 401, Message = "Недостаточно прав" }));

        return Ok(ApiResponse<SharedPlaylistDto>.Ok(_sharedService.MapToDto(entity)));
    }

    // GET /api/sharedplaylist/{token}/tracks
    [HttpGet("{token}/tracks")]
    public async Task<ActionResult<ApiResponse<YandexPlaylistData>>> GetTracks(string token)
    {
        var currentUserId = User.GetUserIdOrNull();
        var playlist = await _sharedService.GetEntityByTokenAsync(token);
        if (playlist == null)
            return NotFound(ApiResponse<YandexPlaylistData>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (!_sharedService.CanView(playlist, currentUserId))
            return Unauthorized();

        var creator = await _userManager.FindByIdAsync(playlist.CreatorUserId.ToString());
        if (creator == null)
            return StatusCode(500, ApiResponse<YandexPlaylistData>.Fail(new ErrorResponse { StatusCode = 500, Message = "Владелец плейлиста не найден" }));

        var dto = await _yandexService.GetPlaylistDataAsync(creator, playlist.YandexPlaylistOwnerUid, playlist.YandexPlaylistKind);
        if (dto == null)
            return NotFound(ApiResponse<YandexPlaylistData>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден в Яндекс.Музыке" }));

        return Ok(ApiResponse<YandexPlaylistData>.Ok(dto));
    }

    // PUT /api/sharedplaylist/{token}/permissions
    [HttpPut("{token}/permissions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SharedPlaylistDto>>> UpdatePermissions(string token, [FromBody] UpdatePermissionsDto dto)
    {
        var userId = User.GetUserId();
        var playlist = await _sharedService.GetEntityByTokenAsync(token);
        if (playlist == null)
            return NotFound(ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (playlist.CreatorUserId != userId)
            return Forbid();

        var updated = await _sharedService.UpdatePermissionsAsync(playlist.Id, dto);
        if (updated == null)
            return BadRequest(ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { StatusCode = 400, Message = "Ошибка обновления прав" }));

        return Ok(ApiResponse<SharedPlaylistDto>.Ok(updated));
    }

    // POST /api/sharedplaylist/{token}/add-tracks
    [HttpPost("{token}/add-tracks")]
    public async Task<ActionResult<ApiResponse<object>>> AddTracks(string token, [FromBody] UpdateTrackListRequest request)
    {
        var currentUserId = User.GetUserIdOrNull();
        var playlist = await _sharedService.GetEntityByTokenAsync(token);
        if (playlist == null)
            return NotFound(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (!_sharedService.CanAddTrack(playlist, currentUserId))
            return StatusCode(403, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 403, Message = "Недостаточно прав для добавления треков" }));

        var creator = await _userManager.FindByIdAsync(playlist.CreatorUserId.ToString());
        if (creator == null)
            return StatusCode(500, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 500, Message = "Владелец плейлиста не найден" }));

        var updatedPlaylist = await _yandexService.AddTracksAsync(creator, playlist.YandexPlaylistOwnerUid, playlist.YandexPlaylistKind, request.TrackIds);
        if (updatedPlaylist == null)
            return StatusCode(500, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 500, Message = "Ошибка при добавлении треков" }));

        var session = await _userSessionService.GetOrCreateCurrentSessionAsync(currentUserId);
        var sessionId = session.SessionId;
        foreach (var trackId in request.TrackIds)
        {
            await _trackAdditionLogService.LogAdditionAsync(playlist.Id, trackId, currentUserId, sessionId);
        }

        return Ok(ApiResponse<object>.Ok(new { message = "Треки добавлены" }));
    }

    // GET /api/sharedplaylist/{token}/additions
    [HttpGet("{token}/additions")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, string?>>>> GetAdditions(string token)
    {
        var currentUserId = User.GetUserIdOrNull();
        var playlist = await _sharedService.GetEntityByTokenAsync(token);
        if (playlist == null)
            return NotFound(ApiResponse<Dictionary<string, string?>>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        if (!_sharedService.CanView(playlist, currentUserId))
            return Unauthorized();

        var additions = await _trackAdditionLogService.GetAdditionUserNamesAsync(playlist.Id);
        return Ok(ApiResponse<Dictionary<string, string?>>.Ok(additions));
    }

    // POST /api/sharedplaylist/{token}/remove-tracks
    [HttpPost("{token}/remove-tracks")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveTracks(string token, [FromBody] UpdateTrackListRequest request)
    {
        var currentUserId = User.GetUserIdOrNull();
        var playlist = await _sharedService.GetEntityByTokenAsync(token);
        if (playlist == null)
            return NotFound(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        var session = await _userSessionService.GetOrCreateCurrentSessionAsync(currentUserId);
        var sessionId = session.SessionId;

        foreach (var trackId in request.TrackIds)
        {
            if (!await _sharedService.CanRemoveTrackAsync(playlist, currentUserId, trackId, sessionId))
                return StatusCode(403, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 403, Message = $"Недостаточно прав для удаления трека {trackId}" }));
        }

        var creator = await _userManager.FindByIdAsync(playlist.CreatorUserId.ToString());
        if (creator == null)
            return StatusCode(500, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 500, Message = "Владелец плейлиста не найден" }));

        var updatedPlaylist = await _yandexService.RemoveTracksAsync(creator, playlist.YandexPlaylistOwnerUid, playlist.YandexPlaylistKind, request.TrackIds);
        if (updatedPlaylist == null)
            return StatusCode(500, ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 500, Message = "Ошибка при удалении треков" }));

        foreach (var trackId in request.TrackIds)
        {
            await _trackRemovalLogService.LogRemovalAsync(playlist.Id, trackId, currentUserId, sessionId);
            await _trackAdditionLogService.RemoveLogsForTrackAsync(playlist.Id, trackId);
        }

        return Ok(ApiResponse<object>.Ok(new { message = "Треки удалены" }));
    }

}