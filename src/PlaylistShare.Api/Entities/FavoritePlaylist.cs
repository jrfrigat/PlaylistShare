namespace PlaylistShare.Api.Entities;

/// <summary>Избранный расшаренный плейлист пользователя.</summary>
public class FavoritePlaylist
{
    public Guid UserId { get; set; }
    public Guid SharedPlaylistId { get; set; }
    public DateTime AddedAtUtc { get; set; }

    // Навигационные свойства
    public ApplicationUser User { get; set; } = null!;
    public SharedPlaylist SharedPlaylist { get; set; } = null!;
}