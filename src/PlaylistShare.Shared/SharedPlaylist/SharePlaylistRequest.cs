using System.ComponentModel.DataAnnotations;

namespace PlaylistShare.Shared.SharedPlaylist;

/// <summary>Запрос на открытие доступа к плейлисту Яндекс.Музыки.</summary>
public class SharePlaylistRequest
{
    /// <summary>Идентификатор плейлиста в Яндекс.Музыке (kind).</summary>
    // 50 - ширина колонки SharedPlaylists.YandexPlaylistKind.
    [Required]
    [MaxLength(50)]
    public string Kind { get; set; } = string.Empty;

    /// <summary>Идентификатор владельца плейлиста в Яндекс.Музыке (uid).</summary>
    // 50 - ширина колонки SharedPlaylists.YandexPlaylistOwnerUid.
    [Required]
    [MaxLength(50)]
    public string OwnerUid { get; set; } = string.Empty;
}