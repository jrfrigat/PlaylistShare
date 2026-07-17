using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaylistShare.Api.Entities;

[Table("UserSessions")]
[Index(nameof(AssociatedUserId))]
public class UserSession
{
    [Key]
    [MaxLength(449)]
    public string SessionId { get; set; } = null!;        // HttpContext.Session.Id
    public string? ClientIpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime FirstSeenUtc { get; set; }
    public DateTime LastSeenUtc { get; set; }
    public Guid? AssociatedUserId { get; set; }            // если позже залогинился

    [ForeignKey(nameof(AssociatedUserId))]
    public ApplicationUser? User { get; set; }

    [InverseProperty(nameof(TrackAdditionLog.Session))]
    public ICollection<TrackAdditionLog> TrackAdditionLogs { get; set; } = new List<TrackAdditionLog>();

    [InverseProperty(nameof(TrackRemovalLog.Session))]
    public ICollection<TrackRemovalLog> TrackRemovalLogs { get; set; } = new List<TrackRemovalLog>();
}
