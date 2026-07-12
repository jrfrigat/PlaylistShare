namespace PlaylistShare.Api.Entities;

/// <summary>
/// Persisted marker for a QR sign-in attempt: it stores only the QR link and lifecycle timestamps.
/// The live attempt state (Passport session cookies + client) is held in-memory (IMemoryCache) for
/// its short lifetime - see YandexAuthService.
/// </summary>
public class YandexAuthSession
{
    public int Id { get; set; }
    public Guid? UserId { get; set; }
    public string QrCodeUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public bool IsConfirmed { get; set; }

    public ApplicationUser? User { get; set; }
}
