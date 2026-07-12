using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Auth;

/// <summary>Запрос на регистрацию нового пользователя.</summary>
public class RegisterRequest
{
    /// <summary>Имя пользователя (логин).</summary>
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    /// <summary>Email пользователя.</summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    /// <summary>Пароль.</summary>
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}