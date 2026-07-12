using Microsoft.EntityFrameworkCore;
using PlaylistShare.Api.Data;
using PlaylistShare.Api.Entities;
using PlaylistShare.Shared.SharedPlaylist;

namespace PlaylistShare.Api.Services;

public class FavoritesService
{
    private readonly ApplicationDbContext _db;
    private readonly SharedPlaylistService _sharedPlaylistService;
    private readonly YandexMusicService _yandexService;

    public FavoritesService(ApplicationDbContext db, SharedPlaylistService sharedPlaylistService, YandexMusicService yandexService)
    {
        _db = db;
        _sharedPlaylistService = sharedPlaylistService;
        _yandexService = yandexService;
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, string shareToken)
    {
        var playlist = await _sharedPlaylistService.GetEntityByTokenAsync(shareToken);
        if (playlist == null) return false;
        return await _db.FavoritePlaylists
            .AnyAsync(f => f.UserId == userId && f.SharedPlaylistId == playlist.Id);
    }

    public async Task AddFavoriteAsync(Guid userId, string shareToken)
    {
        var playlist = await _sharedPlaylistService.GetEntityByTokenAsync(shareToken);
        if (playlist == null)
            throw new ArgumentException("Playlist not found");

        var exists = await _db.FavoritePlaylists
            .AnyAsync(f => f.UserId == userId && f.SharedPlaylistId == playlist.Id);
        if (exists) return;

        var favorite = new FavoritePlaylist
        {
            UserId = userId,
            SharedPlaylistId = playlist.Id,
            AddedAtUtc = DateTime.UtcNow
        };
        _db.FavoritePlaylists.Add(favorite);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveFavoriteAsync(Guid userId, string shareToken)
    {
        var playlist = await _sharedPlaylistService.GetEntityByTokenAsync(shareToken);
        if (playlist == null) return;

        var favorite = await _db.FavoritePlaylists
            .FirstOrDefaultAsync(f => f.UserId == userId && f.SharedPlaylistId == playlist.Id);
        if (favorite != null)
        {
            _db.FavoritePlaylists.Remove(favorite);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<SharedPlaylistDto>> GetUserFavoritesAsync(Guid userId)
    {
        var favoritePlaylists = await _db.FavoritePlaylists
            .Include(f => f.SharedPlaylist)
                .ThenInclude(sp => sp.Creator)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.AddedAtUtc)
            .Select(f => f.SharedPlaylist)
            .ToListAsync();

        // Backfill: плейлисты, расшаренные до сохранения обложки, лежат в БД с CoverUrl = null.
        // Подтягиваем обложку из Яндекса токеном создателя и сохраняем - далее берётся из БД.
        var changed = false;
        foreach (var sp in favoritePlaylists)
        {
            if (!string.IsNullOrEmpty(sp.CoverUrl) || sp.Creator == null)
                continue;
            try
            {
                var cover = await _yandexService.GetPlaylistCoverUrlAsync(sp.Creator, sp.YandexPlaylistOwnerUid, sp.YandexPlaylistKind);
                if (!string.IsNullOrEmpty(cover))
                {
                    sp.CoverUrl = cover;
                    changed = true;
                }
            }
            catch { /* обложка не критична - при ошибке просто оставляем заглушку */ }
        }
        if (changed)
            await _db.SaveChangesAsync();

        // Маппинг в DTO (можно использовать AutoMapper, но для простоты сделаем вручную)
        return favoritePlaylists.Select(sp => new SharedPlaylistDto
        {
            Id = sp.Id,
            CreatorUserId = sp.CreatorUserId,
            YandexPlaylistUuid = sp.YandexPlaylistUuid,
            YandexPlaylistKind = sp.YandexPlaylistKind,
            YandexPlaylistOwnerUid = sp.YandexPlaylistOwnerUid,
            Title = sp.Title,
            Description = sp.Description,
            CoverUrl = sp.CoverUrl,
            CreatedAt = sp.CreatedAt,
            UpdatedAt = sp.UpdatedAt,
            IsDeleted = sp.IsDeleted,
            ShareToken = sp.ShareToken,
            ViewPermission = sp.ViewPermission,
            PlayPermission = sp.PlayPermission,
            AddPermission = sp.AddPermission,
            RemovePermission = sp.RemovePermission,
            Creator = sp.Creator != null ? new Shared.Auth.ApplicationUserDto
            {
                Id = sp.Creator.Id,
                UserName = sp.Creator.UserName ?? "",
                Email = sp.Creator.Email ?? "",
                YandexId = sp.Creator.YandexId,
                DisplayName = sp.Creator.UserName ?? ""
            } : null
        }).ToList();
    }
}