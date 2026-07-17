using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Auth;

/// <summary>Запрос на регистрацию нового пользователя.</summary>
public class RegisterRequest
{
    /// <summary>Имя пользователя (логин).</summary>
    // 256 - ширина колонки AspNetUsers.UserName (nvarchar(256), дефолт Identity).
    [Required]
    [MaxLength(256)]
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    /// <summary>Email пользователя.</summary>
    // 256 - ширина колонки AspNetUsers.Email (nvarchar(256), дефолт Identity).
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    /// <summary>Пароль.</summary>
    // 6 - Password.RequiredLength из настроек Identity в Program.cs.
    // Верхней границы нет: пароль хранится хешем фиксированной длины.
    [Required]
    [MinLength(6)]
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}