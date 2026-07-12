using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Auth;

/// <summary>Запрос на вход по паролю.</summary>
public class LoginRequest
{
    /// <summary>Имя пользователя (логин).</summary>
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    /// <summary>Пароль.</summary>
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;

    /// <summary>Запомнить пользователя (продлить сессию).</summary>
    [JsonPropertyName("rememberMe")]
    public bool RememberMe { get; set; }
}