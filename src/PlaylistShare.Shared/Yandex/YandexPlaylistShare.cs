using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Yandex;

/// <summary>Информация о плейлисте из Яндекс.Музыки с пометкой о шаринге.</summary>
public class YandexPlaylistShare : YandexPlaylist
{

    /// <summary>Расшаренный</summary>
    [JsonPropertyName("isShared")]
    public bool IsShared { get; set; }

    /// <summary>Расшаренная ссылка</summary>
    [JsonPropertyName("shareToken")]
    public string? ShareToken { get; set; }
}
