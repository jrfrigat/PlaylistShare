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

    public async Task<bool> IsTrackAddedByCurrentUserOrSessionAsync(Guid sharedPlaylistId, string trackId, Guid? userId, string sessionId, CancellationToken cancellationToken = default)
    {
        return await _db.TrackAdditionLogs
            .AnyAsync(l => l.SharedPlaylistId == sharedPlaylistId && l.TrackId == trackId &&
                (userId != null ? l.AddedByUserId == userId : l.SessionId == sessionId), cancellationToken);
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