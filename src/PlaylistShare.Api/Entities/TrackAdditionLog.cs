namespace PlaylistShare.Api.Entities;

/// <summary>Лог добавления трека.</summary>
public class TrackAdditionLog
{
    public Guid Id { get; set; }
    public Guid SharedPlaylistId { get; set; }
    public string TrackId { get; set; } = null!;
    public Guid? AddedByUserId { get; set; }
    public DateTime AddedAtUtc { get; set; }
    public string SessionId { get; set; } = null!;

    public SharedPlaylist SharedPlaylist { get; set; } = null!;
    public ApplicationUser? AddedByUser { get; set; }
    public UserSession Session { get; set; } = null!;
}