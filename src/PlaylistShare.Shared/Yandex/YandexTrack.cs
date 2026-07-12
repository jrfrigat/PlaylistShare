using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Yandex;

/// <summary>Результат поиска трека в Яндекс.Музыке.</summary>
public class YandexTrack
{
    [JsonPropertyName("trackId")]
    public string TrackId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("artists")]
    public List<YandexArtist> Artists { get; set; } = new();

    [JsonPropertyName("coverUri")]
    public string CoverUri { get; set; } = string.Empty;

    [JsonPropertyName("durationMs")]
    public long DurationMs { get; set; }
}