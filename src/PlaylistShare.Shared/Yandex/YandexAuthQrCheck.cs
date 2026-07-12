using PlaylistShare.Shared.Enums;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Yandex;

/// <summary>Результат авторизации QR</summary>
public class YandexAuthQrCheck
{
    [JsonPropertyName("status")]
    public YandexAuthQrStatus Status { get; set; } = YandexAuthQrStatus.Pending;
}
