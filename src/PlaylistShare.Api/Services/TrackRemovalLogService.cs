using PlaylistShare.Api.Data;

public class TrackRemovalLogService
{
    private readonly ApplicationDbContext _db;
    public TrackRemovalLogService(ApplicationDbContext db) => _db = db;

    public async Task LogRemovalAsync(Guid sharedPlaylistId, string trackId, Guid? removedByUserId, string sessionId)
    {
        var log = new TrackRemovalLog
        {
            Id = Guid.NewGuid(),
            SharedPlaylistId = sharedPlaylistId,
            TrackId = trackId,
            RemovedByUserId = removedByUserId,
            RemovedAtUtc = DateTime.UtcNow,
            SessionId = sessionId
        };
        _db.TrackRemovalLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}