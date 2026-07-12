using Microsoft.EntityFrameworkCore;
using PlaylistShare.Api.Data;
using PlaylistShare.Api.Entities;
using PlaylistShare.Shared.Auth;
using PlaylistShare.Shared.Enums;
using PlaylistShare.Shared.SharedPlaylist;

namespace PlaylistShare.Api.Services;

public class SharedPlaylistService
{
    private readonly ApplicationDbContext _db;
    private readonly TrackAdditionLogService _trackLogService;

    public SharedPlaylistService(ApplicationDbContext db, TrackAdditionLogService trackLogService)
    {
        _db = db;
        _trackLogService = trackLogService;
    }

    public async Task<SharedPlaylistDto> CreateAsync(Guid creatorUserId, SharePlaylistDto dto)
    {
        var entity = new SharedPlaylist
        {
            Id = Guid.NewGuid(),
            CreatorUserId = creatorUserId,
            YandexPlaylistUuid = dto.YandexPlaylistUuid,
            YandexPlaylistKind = dto.YandexPlaylistKind,
            YandexPlaylistOwnerUid = dto.YandexPlaylistOwnerUid,
            Title = dto.Title,
            Description = dto.Description,
            CoverUrl = dto.CoverUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ShareToken = GenerateToken(),
            PlayPermission = dto.PlayPermission,
            ViewPermission = dto.ViewPermission,
            AddPermission = dto.AddPermission,
            RemovePermission = dto.RemovePermission
        };
        _db.SharedPlaylists.Add(entity);
        await _db.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<SharedPlaylist?> GetEntityByTokenAsync(string token)
    {
        return await _db.SharedPlaylists
            .Include(sp => sp.Creator)
            .FirstOrDefaultAsync(sp => sp.ShareToken == token && !sp.IsDeleted);
    }

    public async Task<SharedPlaylistDto?> UpdatePermissionsAsync(Guid playlistId, UpdatePermissionsDto dto)
    {
        var entity = await _db.SharedPlaylists.FindAsync(playlistId);
        if (entity == null) return null;
        entity.ViewPermission = dto.ViewPermission;
        entity.PlayPermission = dto.PlayPermission;
        entity.AddPermission = dto.AddPermission;
        entity.RemovePermission = dto.RemovePermission;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid playlistId)
    {
        var entity = await _db.SharedPlaylists.FindAsync(playlistId);
        if (entity == null) return false;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // Permission checks below are pure in-memory evaluations of the playlist's permission flags, so
    // they are synchronous. Only CanRemoveTrackAsync needs the database (addition-log lookup).
    public bool CanPlay(SharedPlaylist playlist, Guid? currentUserId)
    {
        if (currentUserId == playlist.CreatorUserId) return true;
        return playlist.PlayPermission == ViewPermission.Everyone ||
               (playlist.PlayPermission == ViewPermission.AuthorizedOnly && currentUserId.HasValue);
    }

    public bool CanPlayEveryone(SharedPlaylist playlist) =>
        playlist.PlayPermission == ViewPermission.Everyone;

    public bool CanView(SharedPlaylist playlist, Guid? currentUserId)
    {
        if (currentUserId == playlist.CreatorUserId) return true;
        return playlist.ViewPermission == ViewPermission.Everyone ||
               (playlist.ViewPermission == ViewPermission.AuthorizedOnly && currentUserId.HasValue);
    }

    public bool CanAddTrack(SharedPlaylist playlist, Guid? currentUserId)
    {
        if (currentUserId == playlist.CreatorUserId) return true;
        return playlist.AddPermission == EditPermission.Everyone ||
               (playlist.AddPermission == EditPermission.AuthorizedOnly && currentUserId.HasValue);
    }

    public async Task<bool> CanRemoveTrackAsync(SharedPlaylist playlist, Guid? currentUserId, string trackId, string sessionId)
    {
        if (currentUserId == playlist.CreatorUserId) return true;
        return playlist.RemovePermission switch
        {
            EditPermission.Everyone => true,
            EditPermission.AuthorizedOnly => currentUserId.HasValue,
            EditPermission.AddedByUserOnly when currentUserId.HasValue =>
                await _trackLogService.IsTrackAddedByCurrentUserOrSessionAsync(playlist.Id, trackId, currentUserId, sessionId),
            EditPermission.AddedByUserOnly when !currentUserId.HasValue =>
                await _trackLogService.IsTrackAddedByCurrentUserOrSessionAsync(playlist.Id, trackId, null, sessionId),
            _ => false
        };
    }

    public async Task<bool> IsCreatorAsync(Guid playlistId, Guid userId)
    {
        var playlist = await _db.SharedPlaylists.FindAsync(playlistId);
        return playlist != null && playlist.CreatorUserId == userId;
    }

    private string GenerateToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .TrimEnd('=');
    }

    public async Task<List<SharedPlaylist>> GetAllByUserAsync(Guid userId)
    {
        return await _db.SharedPlaylists
            .Where(sp => sp.CreatorUserId == userId && !sp.IsDeleted)
            .ToListAsync();
    }

    public SharedPlaylistDto MapToDto(SharedPlaylist entity)
    {
        return new SharedPlaylistDto
        {
            Id = entity.Id,
            CreatorUserId = entity.CreatorUserId,
            YandexPlaylistUuid = entity.YandexPlaylistUuid,
            YandexPlaylistKind = entity.YandexPlaylistKind,
            YandexPlaylistOwnerUid = entity.YandexPlaylistOwnerUid,
            Title = entity.Title,
            Description = entity.Description,
            CoverUrl = entity.CoverUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsDeleted = entity.IsDeleted,
            ShareToken = entity.ShareToken,
            ViewPermission = entity.ViewPermission,
            PlayPermission = entity.PlayPermission,
            AddPermission = entity.AddPermission,
            RemovePermission = entity.RemovePermission,
            Creator = entity.Creator != null ? new ApplicationUserDto
            {
                Id = entity.Creator.Id,
                UserName = entity.Creator.UserName ?? string.Empty,
                Email = entity.Creator.Email,
                YandexId = entity.Creator.YandexId,
                DisplayName = entity.Creator.UserName
            } : null
        };
    }
}