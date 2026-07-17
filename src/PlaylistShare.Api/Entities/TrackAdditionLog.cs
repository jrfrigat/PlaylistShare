using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaylistShare.Api.Entities;

/// <summary>Лог добавления трека.</summary>
[Table("TrackAdditionLogs")]
[Index(nameof(SharedPlaylistId), nameof(TrackId))]
public class TrackAdditionLog
{
    [Key]
    public Guid Id { get; set; }
    public Guid SharedPlaylistId { get; set; }
    public string TrackId { get; set; } = null!;
    public Guid? AddedByUserId { get; set; }
    public DateTime AddedAtUtc { get; set; }
    public string SessionId { get; set; } = null!;

    [ForeignKey(nameof(SharedPlaylistId))]
    [InverseProperty(nameof(Entities.SharedPlaylist.TrackAdditionLogs))]
    public SharedPlaylist SharedPlaylist { get; set; } = null!;

    [ForeignKey(nameof(AddedByUserId))]
    public ApplicationUser? AddedByUser { get; set; }

    [ForeignKey(nameof(SessionId))]
    [InverseProperty(nameof(UserSession.TrackAdditionLogs))]
    public UserSession Session { get; set; } = null!;
}
