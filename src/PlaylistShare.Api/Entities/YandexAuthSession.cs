using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaylistShare.Api.Entities;

/// <summary>
/// Persisted marker for a QR sign-in attempt: it stores only the QR link and lifecycle timestamps.
/// The live attempt state (Passport session cookies + client) is held in-memory (IMemoryCache) for
/// its short lifetime - see YandexAuthService.
/// </summary>
[Table("YandexAuthSessions")]
[Index(nameof(UserId))]
public class YandexAuthSession
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Guid? UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string QrCodeUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public bool IsConfirmed { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }
}
