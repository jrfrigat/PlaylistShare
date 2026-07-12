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
[Authorize]
public class PlaylistsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SharedPlaylistService _sharedService;
    private readonly YandexMusicService _yandexService;

    public PlaylistsController(
        UserManager<ApplicationUser> userManager,
        SharedPlaylistService sharedService,
        YandexMusicService yandexService)
    {
        _userManager = userManager;
        _sharedService = sharedService;
        _yandexService = yandexService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<YandexPlaylistShare>>>> GetMyPlaylists()
    {
        var userId = User.GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Unauthorized();

        if (string.IsNullOrEmpty(user.YandexAccessToken))
            return BadRequest(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 400, Message = "Токен Яндекс.Музыки не установлен или недействителен" }));

        List<YandexPlaylistShare> result;
        try
        {
            var (ownPlaylists, _) = await _yandexService.GetOwnFavoritesAsync(user);
            var sharedPlaylists = await _sharedService.GetAllByUserAsync(userId);

            // Index the user's shared playlists by (kind, ownerUid) once so the projection below is a
            // single dictionary lookup per playlist instead of two linear scans of the shared list.
            var sharedByKey = sharedPlaylists
                .GroupBy(s => (s.YandexPlaylistKind, s.YandexPlaylistOwnerUid))
                .ToDictionary(g => g.Key, g => g.First());

            result = (ownPlaylists ?? []).Select(p =>
            {
                var kind = p.Kind.ToString();
                var ownerUid = p.Owner.Uid.ToString();
                sharedByKey.TryGetValue((kind, ownerUid), out var shared);
                return new YandexPlaylistShare
                {
                    Kind = kind,
                    OwnerUid = ownerUid,
                    Title = p.Title,
                    CoverUrl = p.Cover?.GetUrl() ?? "",
                    TrackCount = p.TrackCount,
                    IsShared = shared != null,
                    ShareToken = shared?.ShareToken,
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 400, Message = ex.Message }));
        }

        return Ok(ApiResponse<List<YandexPlaylistShare>>.Ok(result));
    }

    [HttpPost("share")]
    public async Task<ActionResult<ApiResponse<SharedPlaylistDto>>> SharePlaylist([FromBody] SharePlaylistRequest request)
    {
        var userId = User.GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Unauthorized();

        var playlist = await _yandexService.GetPlaylistAsync(user, request.OwnerUid, request.Kind);
        if (playlist == null)
            return BadRequest(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        var dto = new SharePlaylistDto
        {
            YandexPlaylistUuid = playlist.PlaylistUuid ?? string.Empty,
            YandexPlaylistKind = request.Kind,
            YandexPlaylistOwnerUid = request.OwnerUid,
            Title = playlist.Title,
            Description = playlist.Description,
            CoverUrl = playlist.Cover?.GetUrl(),
            ViewPermission = Shared.Enums.ViewPermission.Everyone,
            PlayPermission = Shared.Enums.ViewPermission.Everyone,
            AddPermission = Shared.Enums.EditPermission.AuthorizedOnly,
            RemovePermission = Shared.Enums.EditPermission.AddedByUserOnly,
        };

        var result = await _sharedService.CreateAsync(userId, dto);
        return Ok(ApiResponse<SharedPlaylistDto>.Ok(result));
    }
}
