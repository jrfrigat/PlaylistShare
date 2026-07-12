namespace PlaylistShare.Api.Entities;

public class UserSession
{
    public string SessionId { get; set; } = null!;        // HttpContext.Session.Id
    public string? ClientIpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime FirstSeenUtc { get; set; }
    public DateTime LastSeenUtc { get; set; }
    public Guid? AssociatedUserId { get; set; }            // если позже залогинился

    public ApplicationUser? User { get; set; }
    public ICollection<TrackAdditionLog> TrackAdditionLogs { get; set; } = new List<TrackAdditionLog>();
    public ICollection<TrackRemovalLog> TrackRemovalLogs { get; set; } = new List<TrackRemovalLog>();
}
