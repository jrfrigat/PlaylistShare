using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Enums;

/// <summary>Кто может просматривать плейлист.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ViewPermission
{
    /// <summary>Все, включая неавторизованных.</summary>
    Everyone,

    /// <summary>Только авторизованные пользователи.</summary>
    AuthorizedOnly,
}
