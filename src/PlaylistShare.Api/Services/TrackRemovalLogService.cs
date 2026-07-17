using PlaylistShare.Api.Data;

public class TrackRemovalLogService
{
    // DI-owned (scoped) ApplicationDbContext - the container manages its lifetime.
    // Do NOT implement IDisposable/dispose _db here, that would tear down a context
    // shared by the rest of the request scope.
    private readonly ApplicationDbContext _db;
    public TrackRemovalLogService(ApplicationDbContext db) => _db = db;

    public async Task LogRemovalAsync(Guid sharedPlaylistId, string trackId, Guid? removedByUserId, string sessionId, CancellationToken cancellationToken = default)
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
        await _db.SaveChangesAsync(cancellationToken);
    }
}