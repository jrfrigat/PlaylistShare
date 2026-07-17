using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlaylistShare.Api.Extensions;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared;
using PlaylistShare.Shared.SharedPlaylist;

namespace PlaylistShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FavoritesService _favoritesService;
    private readonly SharedPlaylistService _sharedPlaylistService;

    public FavoritesController(
        FavoritesService favoritesService,
        SharedPlaylistService sharedPlaylistService)
    {
        _favoritesService = favoritesService;
        _sharedPlaylistService = sharedPlaylistService;
    }

    /// <summary>Получить список избранных плейлистов текущего пользователя.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SharedPlaylistDto>>>> GetFavorites(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var favorites = await _favoritesService.GetUserFavoritesAsync(userId, cancellationToken);
        return Ok(ApiResponse<List<SharedPlaylistDto>>.Ok(favorites));
    }

    /// <summary>Проверить, добавлен ли плейлист в избранное.</summary>
    [HttpGet("{shareToken}/check")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckFavorite(string shareToken, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var isFavorite = await _favoritesService.IsFavoriteAsync(userId, shareToken, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(isFavorite));
    }

    /// <summary>Добавить плейлист в избранное.</summary>
    [HttpPost("{shareToken}")]
    public async Task<ActionResult<ApiResponse<object>>> AddFavorite(string shareToken, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var playlist = await _sharedPlaylistService.GetEntityByTokenAsync(shareToken, cancellationToken);
        if (playlist == null)
            return NotFound(ApiResponse<object>.Fail(new ErrorResponse { StatusCode = 404, Message = "Плейлист не найден" }));

        await _favoritesService.AddFavoriteAsync(userId, shareToken, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { message = "Плейлист добавлен в избранное" }));
    }

    /// <summary>Удалить плейлист из избранного.</summary>
    [HttpDelete("{shareToken}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveFavorite(string shareToken, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _favoritesService.RemoveFavoriteAsync(userId, shareToken, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { message = "Плейлист удалён из избранного" }));
    }
}
