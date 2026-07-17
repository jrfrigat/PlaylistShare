using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaylistShare.Database.Entities;

/// <summary>Избранный расшаренный плейлист пользователя.</summary>
[Table("FavoritePlaylists")]
[PrimaryKey(nameof(UserId), nameof(SharedPlaylistId))]
public class FavoritePlaylist
{
    public Guid UserId { get; set; }
    public Guid SharedPlaylistId { get; set; }
    public DateTime AddedAtUtc { get; set; }

    // Навигационные свойства
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(ApplicationUser.FavoritePlaylists))]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(SharedPlaylistId))]
    public SharedPlaylist SharedPlaylist { get; set; } = null!;
}
