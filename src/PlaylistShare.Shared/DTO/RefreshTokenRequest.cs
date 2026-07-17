using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.DTO;

/// <summary>Запрос на обновление JWT токена.</summary>
public class RefreshTokenRequest
{
    /// <summary>Refresh токен, полученный при входе.</summary>
    // 36 - длина Guid в формате "D": JwtService выдаёт refresh токен как Guid.NewGuid().ToString().
    // Колонка AspNetUsers.RefreshToken это nvarchar(max), так что ограничение идёт от генератора,
    // а не от схемы: более длинная строка заведомо не совпадёт ни с одним выданным токеном.
    [Required]
    [MaxLength(36)]
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = null!;
}