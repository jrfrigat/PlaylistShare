using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum YandexAuthQrStatus
{
    Pending,
    Authorized,
    Expired,
    Error,
}