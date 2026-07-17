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

    /// <summary>
    /// HttpContext.Session.Id - им отличаем анонимов друг от друга (AddedByUserOnly).
    /// Длину задаём явно: раньше её навязывал внешний ключ на UserSessions, и без неё колонка
    /// стала бы nvarchar(max)/text - незачем переписывать журнал ради строки в 36 символов.
    /// </summary>
    [MaxLength(449)]
    public string SessionId { get; set; } = null!;

    [ForeignKey(nameof(SharedPlaylistId))]
    public SharedPlaylist SharedPlaylist { get; set; } = null!;

    [ForeignKey(nameof(RemovedByUserId))]
    public ApplicationUser? RemovedByUser { get; set; }
}
