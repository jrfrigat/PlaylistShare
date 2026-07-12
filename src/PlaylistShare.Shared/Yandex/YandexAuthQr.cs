using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Yandex;

/// <summary>Результат авторизации QR</summary>
public class YandexAuthQr
{
    [JsonPropertyName("qrLink")]
    public string QrLink { get; set; } = string.Empty;

    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>Короткий код, который пользователь вводит на странице подтверждения (device-code flow).</summary>
    [JsonPropertyName("userCode")]
    public string UserCode { get; set; } = string.Empty;
}