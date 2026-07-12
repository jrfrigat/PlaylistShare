using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Yandex;

/// <summary>Информация о плейлисте из Яндекс.Музыки (для импорта).</summary>
public class YandexSearchResult
{
    /// <summary>
    /// Найденные треки.
    /// </summary>
    [JsonPropertyName("tracks")]
    public List<YandexTrack>? Tracks { get; set; } = null;

    /// <summary>
    /// Найденные плейлисты.
    /// </summary>
    [JsonPropertyName("playlists")]
    public List<YandexPlaylist>? Playlists { get; set; } = null;

    /// <summary>
    /// Персональные плейлисты.
    /// </summary>
    [JsonPropertyName("personalPlaylists")]
    public List<YandexPlaylist>? PersonalPlaylists { get; set; } = null;

    /// <summary>
    /// Плейлисты, которые понравились.
    /// </summary>
    [JsonPropertyName("likedPlaylists")]
    public List<YandexPlaylist>? LikedPlaylists { get; set; } = null;

    /// <summary>
    /// Найденные исполнители.
    /// </summary>
    [JsonPropertyName("artists")]
    public List<YandexArtist>? Artists { get; set; } = null;

    /// <summary>
    /// Найденные альбомы.
    /// </summary>
    [JsonPropertyName("albumns")]
    public List<YandexAlbum>? Albums { get; set; } = null;
}
