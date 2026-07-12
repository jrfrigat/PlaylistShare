using PlaylistShare.Shared.Enums;

namespace PlaylistShare.Api.Entities;

/// <summary>Сущность шеринг-плейлиста.</summary>
public class SharedPlaylist
{
    public Guid Id { get; set; }
    public Guid CreatorUserId { get; set; }
    public string YandexPlaylistUuid { get; set; } = null!;
    public string YandexPlaylistKind { get; set; } = null!;
    public string YandexPlaylistOwnerUid { get; set; } = null!;
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
    public ApplicationUser Creator { get; set; } = null!;
    public ICollection<TrackAdditionLog> TrackAdditionLogs { get; set; } = new List<TrackAdditionLog>();
}