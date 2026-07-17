using Microsoft.EntityFrameworkCore;
using PlaylistShare.Database;
using PlaylistShare.Database.Entities;
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

    public async Task<SharedPlaylistDto> CreateAsync(Guid creatorUserId, SharePlaylistDto dto, CancellationToken cancellationToken = default)
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
        await _db.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    /// <summary>
    /// Читающая точка входа по share-токену: результат только отображается в DTO или отдаётся как
    /// контекст (Creator - для запросов к Яндексу), но никогда не изменяется и не сохраняется,
    /// поэтому AsNoTracking. Изменяющие методы (UpdatePermissionsAsync, DeleteAsync) грузят строку
    /// заново своим FindAsync и получают отслеживаемую сущность.
    /// </summary>
    public async Task<SharedPlaylist?> GetEntityByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _db.SharedPlaylists
            .AsNoTracking()
            .Include(sp => sp.Creator)
            .FirstOrDefaultAsync(sp => sp.ShareToken == token && !sp.IsDeleted, cancellationToken);
    }

    public async Task<SharedPlaylistDto?> UpdatePermissionsAsync(Guid playlistId, UpdatePermissionsDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.SharedPlaylists.FindAsync([playlistId], cancellationToken);
        if (entity == null) return null;
        entity.ViewPermission = dto.ViewPermission;
        entity.PlayPermission = dto.PlayPermission;
        entity.AddPermission = dto.AddPermission;
        entity.RemovePermission = dto.RemovePermission;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid playlistId, CancellationToken cancellationToken = default)
    {
        var entity = await _db.SharedPlaylists.FindAsync([playlistId], cancellationToken);
        if (entity == null) return false;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
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

    public async Task<bool> CanRemoveTrackAsync(SharedPlaylist playlist, Guid? currentUserId, string trackId, string sessionId, CancellationToken cancellationToken = default)
    {
        if (currentUserId == playlist.CreatorUserId) return true;
        return playlist.RemovePermission switch
        {
            EditPermission.Everyone => true,
            EditPermission.AuthorizedOnly => currentUserId.HasValue,
            EditPermission.AddedByUserOnly =>
                await _trackLogService.IsTrackAddedByCurrentUserOrSessionAsync(playlist.Id, trackId, currentUserId, sessionId, cancellationToken),
            _ => false
        };
    }

    /// <summary>
    /// Пакетный аналог <see cref="CanRemoveTrackAsync"/>: возвращает подмножество trackIds, которые
    /// текущий пользователь вправе удалить. Набор правил обязан совпадать с CanRemoveTrackAsync
    /// ветка в ветку - меняя одно, меняйте и второе.
    ///
    /// Считаем пачкой, а не вызовом CanRemoveTrackAsync в цикле, потому что цикл дал бы по запросу к
    /// журналу на каждый трек: в реальных плейлистах их сотни, и один показ страницы стоил бы сотен
    /// запросов. Everyone / AuthorizedOnly / Nobody отвечают одинаково для всех треков - на этих
    /// ветках в БД не идём совсем; только AddedByUserOnly тянет журнал плейлиста ОДНИМ запросом и
    /// дальше раздаёт признак в памяти.
    ///
    /// sessionIdFactory, а не готовый sessionId: сессия нужна только ветке AddedByUserOnly, а её
    /// получение само стоит запросов - на остальных ветках её не трогаем.
    /// </summary>
    public async Task<HashSet<string>> GetRemovableTrackIdsAsync(
        SharedPlaylist playlist,
        Guid? currentUserId,
        IReadOnlyCollection<string> trackIds,
        Func<CancellationToken, Task<string>> sessionIdFactory,
        CancellationToken cancellationToken = default)
    {
        if (currentUserId == playlist.CreatorUserId) return trackIds.ToHashSet();
        return playlist.RemovePermission switch
        {
            EditPermission.Everyone => trackIds.ToHashSet(),
            EditPermission.AuthorizedOnly => currentUserId.HasValue ? trackIds.ToHashSet() : new HashSet<string>(),
            EditPermission.AddedByUserOnly =>
                await FilterAddedByAsync(playlist.Id, trackIds, currentUserId, await sessionIdFactory(cancellationToken), cancellationToken),
            _ => new HashSet<string>()
        };
    }

    private async Task<HashSet<string>> FilterAddedByAsync(Guid playlistId, IReadOnlyCollection<string> trackIds, Guid? userId, string sessionId, CancellationToken cancellationToken)
    {
        var addedByMe = await _trackLogService.GetTrackIdsAddedByUserOrSessionAsync(playlistId, userId, sessionId, cancellationToken);
        return trackIds.Where(addedByMe.Contains).ToHashSet();
    }

    public async Task<bool> IsCreatorAsync(Guid playlistId, Guid userId, CancellationToken cancellationToken = default)
    {
        var playlist = await _db.SharedPlaylists.FindAsync([playlistId], cancellationToken);
        return playlist != null && playlist.CreatorUserId == userId;
    }

    private string GenerateToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .TrimEnd('=');
    }

    public async Task<List<SharedPlaylist>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.SharedPlaylists
            .AsNoTracking()
            .Where(sp => sp.CreatorUserId == userId && !sp.IsDeleted)
            .ToListAsync(cancellationToken);
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