using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Yandex;

/// <summary>Информация о альбоме из Яндекс.Музыки.</summary>
public class YandexAlbum
{
    /// <summary>Идентификатор альбома (id).</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>Наименование альбома.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    /// <summary>Исполнители альбома.</summary>
    [JsonPropertyName("artists")]
    public List<YandexArtist> Artists { get; set; } = null!;

    /// <summary>Описание альбома.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>URL обложки альбома.</summary>
    [JsonPropertyName("coverUrl")]
    public string? CoverUrl { get; set; }
}
