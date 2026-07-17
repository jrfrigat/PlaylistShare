using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Auth;

/// <summary>Запрос на вход по паролю.</summary>
public class LoginRequest
{
    /// <summary>Имя пользователя (логин).</summary>
    // 256 - ширина колонки AspNetUsers.UserName (nvarchar(256), дефолт Identity).
    [Required]
    [MaxLength(256)]
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    /// <summary>Пароль.</summary>
    // Минимальной длины тут намеренно нет: политика паролей могла меняться,
    // и старый короткий пароль всё равно должен пройти проверку в Identity.
    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;

    /// <summary>Запомнить пользователя (продлить сессию).</summary>
    [JsonPropertyName("rememberMe")]
    public bool RememberMe { get; set; }
}