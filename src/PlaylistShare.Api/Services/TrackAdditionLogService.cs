using Microsoft.EntityFrameworkCore;
using PlaylistShare.Database;
using PlaylistShare.Database.Entities;

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
    /// Трек добавлен этим же человеком? Предикат обязан совпадать с
    /// <see cref="GetTrackIdsAddedByUserOrSessionAsync"/> - меняя одно, меняйте и второе.
    ///
    /// Правило: учётка опознаёт человека надёжно, сессия - только слабый заменитель для АНОНИМА,
    /// у которого учётки нет. Поэтому совпадение сессии засчитывается лишь для треков, добавленных
    /// анонимно (AddedByUserId == null). Если трек добавлен из-под учётки, снять его может только
    /// эта учётка: сессия чужую учётку не перебивает никогда. Иначе на общем компьютере хватило бы
    /// разлогиниться и зайти под собой, чтобы получить чужие треки - серверная сессия у браузера
    /// одна на всех, кто в нём побывал.
    ///
    /// Проверка userId != null внутри первой скобки обязательна, НЕ упрощать до
    /// "l.AddedByUserId == userId": у анонима userId равен null, сравнение выродилось бы в
    /// "AddedByUserId == null" и совпало бы со ВСЕМИ анонимными добавлениями любых чужих сессий.
    ///
    /// Вторая скобка сохраняет нужный случай: аноним добавил трек, потом залогинился - трек остаётся
    /// его, потому что добавлен он был анонимно и из этой же сессии.
    /// SessionId - серверный HttpContext.Session.Id, подделать его клиент не может.
    /// </summary>
    public async Task<bool> IsTrackAddedByCurrentUserOrSessionAsync(Guid sharedPlaylistId, string trackId, Guid? userId, string sessionId, CancellationToken cancellationToken = default)
    {
        return await _db.TrackAdditionLogs
            .AnyAsync(l => l.SharedPlaylistId == sharedPlaylistId && l.TrackId == trackId &&
                ((userId != null && l.AddedByUserId == userId) ||
                 (l.AddedByUserId == null && l.SessionId == sessionId)), cancellationToken);
    }

    /// <summary>
    /// Пакетный аналог <see cref="IsTrackAddedByCurrentUserOrSessionAsync"/>: все треки плейлиста,
    /// добавленные этим же человеком, одним запросом. Предикат тот же, только без фильтра по TrackId -
    /// вызывающий сверяется с множеством в памяти, вместо того чтобы ходить в журнал за каждым треком
    /// по отдельности.
    ///
    /// Про смысл обеих скобок см. IsTrackAddedByCurrentUserOrSessionAsync: учётка опознаёт человека,
    /// сессия - лишь замена учётки для анонима, поэтому чужую учётку она не перебивает. Не упрощать.
    /// </summary>
    public async Task<HashSet<string>> GetTrackIdsAddedByUserOrSessionAsync(Guid sharedPlaylistId, Guid? userId, string sessionId, CancellationToken cancellationToken = default)
    {
        var trackIds = await _db.TrackAdditionLogs
            .AsNoTracking()
            .Where(l => l.SharedPlaylistId == sharedPlaylistId &&
                ((userId != null && l.AddedByUserId == userId) ||
                 (l.AddedByUserId == null && l.SessionId == sessionId)))
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