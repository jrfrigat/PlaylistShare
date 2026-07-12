using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Yandex;

/// <summary>Информация о исполнителе из Яндекс.Музыки.</summary>
public class YandexArtist
{
    /// <summary>Идентификатор исполнителя (id).</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>Наименование исполнителя.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>Описание исполнителя.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>URL исполнителя.</summary>
    [JsonPropertyName("coverUrl")]
    public string? CoverUrl { get; set; }
}
