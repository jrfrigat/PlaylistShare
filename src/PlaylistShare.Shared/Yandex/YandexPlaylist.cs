using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Yandex;

/// <summary>Информация о плейлисте из Яндекс.Музыки.</summary>
public class YandexPlaylist
{
    /// <summary>Идентификатор плейлиста (uuid).</summary>
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = null!;

    /// <summary>Идентификатор плейлиста (kind).</summary>
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = null!;

    /// <summary>Идентификатор владельца плейлиста (uid).</summary>
    [JsonPropertyName("ownerUid")]
    public string OwnerUid { get; set; } = null!;

    /// <summary>Название плейлиста.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    /// <summary>Описание плейлиста.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>URL обложки.</summary>
    [JsonPropertyName("coverUrl")]
    public string? CoverUrl { get; set; }

    /// <summary>Кол-во треков.</summary>
    [JsonPropertyName("trackCount")]
    public int TrackCount { get; set; }
}
