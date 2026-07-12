using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Auth;

/// <summary>Ответ после успешного входа.</summary>
public class LoginResponse
{
    /// <summary>JWT токен доступа.</summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;

    /// <summary>Refresh токен для обновления сессии.</summary>
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = null!;

    /// <summary>Время истечения токена (UTC).</summary>
    [JsonPropertyName("expiration")]
    public DateTime Expiration { get; set; }
}