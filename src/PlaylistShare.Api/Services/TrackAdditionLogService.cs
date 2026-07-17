using Microsoft.EntityFrameworkCore;
using PlaylistShare.Api.Data;
using PlaylistShare.Api.Entities;

namespace PlaylistShare.Api.Services;

public class TrackAdditionLogService
{
    private readonly ApplicationDbContext _db;

    public TrackAdditionLogService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task LogAdditionAsync(Guid sharedPlaylistId, string trackId, Guid? addedByUserId, string sessionId, CancellationToken cancellationToken = default)
    {
        var log = new TrackAdditionLog
        {
            Id = Guid.NewGuid(),
            SharedPlaylistId = sharedPlaylistId,
            TrackId = trackId,
            AddedByUserId = addedByUserId,
            AddedAtUtc = DateTime.UtcNow,
            SessionId = sessionId
        };
        _db.TrackAdditionLogs.Add(log);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Трек добавлен тем же пользователем ИЛИ из той же сессии. Предикат обязан совпадать с
    /// <see cref="GetTrackIdsAddedByUserOrSessionAsync"/> - меняя одно, меняйте и второе.
    ///
    /// Проверка userId != null внутри скобки обязательна, НЕ упрощать до
    /// "l.AddedByUserId == userId || l.SessionId == sessionId": у анонима userId равен null, и тогда
    /// первое сравнение выродилось бы в "AddedByUserId == null", то есть совпало бы со ВСЕМИ
    /// анонимными добавлениями любых чужих сессий - любой аноним удалял бы чужие треки.
    ///
    /// Сравнение по сессии намеренно работает и для авторизованного: аноним добавил трек, потом
    /// залогинился - трек остаётся его, потому что сессия та же. Обратное тоже следует отсюда:
    /// разлогинившись в том же браузере, пользователь всё ещё видит свои добавления как свои.
    /// SessionId - серверный HttpContext.Session.Id, подделать его клиент не может.
    /// </summary>
    public async Task<bool> IsTrackAddedByCurrentUserOrSessionAsync(Guid sharedPlaylistId, string trackId, Guid? userId, string sessionId, CancellationToken cancellationToken = default)
    {
        return await _db.TrackAdditionLogs
            .AnyAsync(l => l.SharedPlaylistId == sharedPlaylistId && l.TrackId == trackId &&
                ((userId != null && l.AddedByUserId == userId) || l.SessionId == sessionId), cancellationToken);
    }

    /// <summary>
    /// Пакетный аналог <see cref="IsTrackAddedByCurrentUserOrSessionAsync"/>: все треки плейлиста,
    /// добавленные этим пользователем или из этой сессии, одним запросом. Предикат тот же, только без
    /// фильтра по TrackId - вызывающий сверяется с множеством в памяти, вместо того чтобы ходить в
    /// журнал за каждым треком по отдельности.
    ///
    /// Про userId != null внутри скобки см. IsTrackAddedByCurrentUserOrSessionAsync: без неё аноним
    /// получил бы все чужие анонимные добавления. Не упрощать.
    /// </summary>
    public async Task<HashSet<string>> GetTrackIdsAddedByUserOrSessionAsync(Guid sharedPlaylistId, Guid? userId, string sessionId, CancellationToken cancellationToken = default)
    {
        var trackIds = await _db.TrackAdditionLogs
            .AsNoTracking()
            .Where(l => l.SharedPlaylistId == sharedPlaylistId &&
                ((userId != null && l.AddedByUserId == userId) || l.SessionId == sessionId))
            .Select(l => l.TrackId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return trackIds.ToHashSet();
    }

    // Без AsNoTracking: строки грузятся именно ради RemoveRange, удалять можно только отслеживаемые.
    public async Task RemoveLogsForTrackAsync(Guid sharedPlaylistId, string trackId, CancellationToken cancellationToken = default)
    {
        var logs = await _db.TrackAdditionLogs
            .Where(l => l.SharedPlaylistId == sharedPlaylistId && l.TrackId == trackId)
            .ToListAsync(cancellationToken);
        _db.TrackAdditionLogs.RemoveRange(logs);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Dictionary<string, string?>> GetAdditionUserNamesAsync(Guid sharedPlaylistId, CancellationToken cancellationToken = default)
    {
        var rows = await _db.TrackAdditionLogs
            .AsNoTracking()
            .Where(l => l.SharedPlaylistId == sharedPlaylistId)
            .Include(l => l.AddedByUser)
            .OrderByDescending(l => l.AddedAtUtc)
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(l => l.TrackId)
            .ToDictionary(g => g.Key, g => g.First().AddedByUser?.UserName);
    }
}