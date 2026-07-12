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

    public async Task LogAdditionAsync(Guid sharedPlaylistId, string trackId, Guid? addedByUserId, string sessionId)
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
        await _db.SaveChangesAsync();
    }

    public async Task<bool> IsTrackAddedByCurrentUserOrSessionAsync(Guid sharedPlaylistId, string trackId, Guid? userId, string sessionId)
    {
        return await _db.TrackAdditionLogs
            .AnyAsync(l => l.SharedPlaylistId == sharedPlaylistId && l.TrackId == trackId &&
                (userId != null ? l.AddedByUserId == userId : l.SessionId == sessionId));
    }

    public async Task RemoveLogsForTrackAsync(Guid sharedPlaylistId, string trackId)
    {
        var logs = await _db.TrackAdditionLogs
            .Where(l => l.SharedPlaylistId == sharedPlaylistId && l.TrackId == trackId)
            .ToListAsync();
        _db.TrackAdditionLogs.RemoveRange(logs);
        await _db.SaveChangesAsync();
    }

    public async Task<Dictionary<string, string?>> GetAdditionUserNamesAsync(Guid sharedPlaylistId)
    {
        var rows = await _db.TrackAdditionLogs
            .Where(l => l.SharedPlaylistId == sharedPlaylistId)
            .Include(l => l.AddedByUser)
            .OrderByDescending(l => l.AddedAtUtc)
            .ToListAsync();

        return rows
            .GroupBy(l => l.TrackId)
            .ToDictionary(g => g.Key, g => g.First().AddedByUser?.UserName);
    }
}