using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlaylistShare.Api.Entities;
using PlaylistShare.Api.Extensions;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared;
using PlaylistShare.Shared.Enums;
using PlaylistShare.Shared.Yandex;

namespace PlaylistShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class YandexSearchController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly YandexMusicService _yandexService;
    private readonly SharedPlaylistService _sharedPlaylistService;

    public YandexSearchController(UserManager<ApplicationUser> userManager, YandexMusicService yandexService, SharedPlaylistService sharedPlaylistService)
    {
        _userManager = userManager;
        _yandexService = yandexService;
        _sharedPlaylistService = sharedPlaylistService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<YandexSearchResult>>> SearchQuery(
        [FromQuery] string query = "",
        [FromQuery] int limit = 40,
        [FromQuery] TrackSearchType searchType = TrackSearchType.All,
        [FromQuery] bool byId = false,
        [FromQuery] string? shared_id = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) && searchType != TrackSearchType.MyPlaylists)
            return BadRequest(ApiResponse<YandexSearchResult>.Fail(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Поисковый запрос не может быть пустым."
            }));

        ApplicationUser? user = null;
        var userId = User.GetUserIdOrNull();
        if (userId.HasValue)
            user = await _userManager.FindByIdAsync(userId.Value.ToString());

        var byShareId = false;

        // Если нет пользователя или у него нет токена, пробуем через shared_id
        if (user == null || string.IsNullOrEmpty(user.YandexAccessToken))
        {
            if (string.IsNullOrEmpty(shared_id))
                return Unauthorized("Не установлен яндекс токен.");

            var playlist = await _sharedPlaylistService.GetEntityByTokenAsync(shared_id, cancellationToken);
            if (playlist == null) return NotFound("Не найден плейлист.");

            if (!_sharedPlaylistService.CanAddTrack(playlist, userId))
                return StatusCode(403, "Нет доступа для добавления трека.");

            var owner = playlist.Creator;
            if (owner == null) return StatusCode(500, "Не удалось найти владельца плейлиста.");
            user = owner;

            byShareId = true;
        }

        if (string.IsNullOrEmpty(user.YandexAccessToken))
            return BadRequest(ApiResponse<YandexSearchResult>.Fail(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Токен Яндекс.Музыки не установлен или недействителен."
            }));

        YandexSearchResult? results = null;

        if (byId)
        {
            results = await _yandexService.SearchTracksByIdAsync(user, query, searchType, cancellationToken);
        }
        else if (searchType == TrackSearchType.MyPlaylists)
        {
            if (byShareId)
            {
                return Unauthorized("Необходимо подключение профиля к яндекс музыке.");
            }

            results = await _yandexService.SearchMyPlaylists(user, cancellationToken);
        }
        else
        {
            results = await _yandexService.SearchAsync(user, query, searchType, limit, cancellationToken);
        }

        return Ok(ApiResponse<YandexSearchResult>.Ok(results));
    }
}