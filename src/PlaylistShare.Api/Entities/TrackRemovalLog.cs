using PlaylistShare.Api.Entities;

public class TrackRemovalLog
{
    public Guid Id { get; set; }
    public Guid SharedPlaylistId { get; set; }
    public string TrackId { get; set; } = null!;
    public Guid? RemovedByUserId { get; set; }
    public DateTime RemovedAtUtc { get; set; }
    public string SessionId { get; set; } = null!;

    public SharedPlaylist SharedPlaylist { get; set; } = null!;
    public ApplicationUser? RemovedByUser { get; set; }
    public UserSession Session { get; set; } = null!;
}