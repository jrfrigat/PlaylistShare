using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Enums;

/// <summary>
/// Типы поиска треков в Яндекс.Музыке, которые можно указать при поисковом запросе.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrackSearchType
{
    All,
    Artist,
    Album,
    Playlist,
    Track,
    MyPlaylists,
}
