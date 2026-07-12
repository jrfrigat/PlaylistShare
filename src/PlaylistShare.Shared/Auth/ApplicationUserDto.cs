using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Auth;

/// <summary>DTO пользователя (без конфиденциальных данных).</summary>
public class ApplicationUserDto
{
    /// <summary>Идентификатор пользователя в системе.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Имя пользователя (логин).</summary>
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = null!;

    /// <summary>Email пользователя.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>Идентификатор пользователя в Яндексе (если привязан).</summary>
    [JsonPropertyName("yandexId")]
    public string? YandexId { get; set; }

    /// <summary>Отображаемое имя (можно использовать UserName).</summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }
}