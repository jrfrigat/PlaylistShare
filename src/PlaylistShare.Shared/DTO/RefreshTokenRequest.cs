using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.DTO;

/// <summary>Запрос на обновление JWT токена.</summary>
public class RefreshTokenRequest
{
    /// <summary>Refresh токен, полученный при входе.</summary>
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = null!;
}