using Microsoft.EntityFrameworkCore;
using PlaylistShare.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaylistShare.Database.Entities;

/// <summary>Сущность шеринг-плейлиста.</summary>
[Table("SharedPlaylists")]
[Index(nameof(ShareToken), IsUnique = true)]
public class SharedPlaylist
{
    [Key]
    public Guid Id { get; set; }
    public Guid CreatorUserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string YandexPlaylistUuid { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string YandexPlaylistKind { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string YandexPlaylistOwnerUid { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public string ShareToken { get; set; } = null!;
    public ViewPermission ViewPermission { get; set; }
    public ViewPermission PlayPermission { get; set; }
    public EditPermission AddPermission { get; set; }
    public EditPermission RemovePermission { get; set; }

    // Навигационные свойства
    [ForeignKey(nameof(CreatorUserId))]
    [InverseProperty(nameof(ApplicationUser.OwnedPlaylists))]
    public ApplicationUser Creator { get; set; } = null!;

    public ICollection<TrackAdditionLog> TrackAdditionLogs { get; set; } = new List<TrackAdditionLog>();
}
