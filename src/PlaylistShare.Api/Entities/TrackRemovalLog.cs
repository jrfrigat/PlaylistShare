using Microsoft.EntityFrameworkCore;
using PlaylistShare.Api.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("TrackRemovalLogs")]
[Index(nameof(SharedPlaylistId), nameof(TrackId))]
public class TrackRemovalLog
{
    [Key]
    public Guid Id { get; set; }
    public Guid SharedPlaylistId { get; set; }
    public string TrackId { get; set; } = null!;
    public Guid? RemovedByUserId { get; set; }
    public DateTime RemovedAtUtc { get; set; }
    public string SessionId { get; set; } = null!;

    [ForeignKey(nameof(SharedPlaylistId))]
    public SharedPlaylist SharedPlaylist { get; set; } = null!;

    [ForeignKey(nameof(RemovedByUserId))]
    public ApplicationUser? RemovedByUser { get; set; }

    [ForeignKey(nameof(SessionId))]
    [InverseProperty(nameof(UserSession.TrackRemovalLogs))]
    public UserSession Session { get; set; } = null!;
}
