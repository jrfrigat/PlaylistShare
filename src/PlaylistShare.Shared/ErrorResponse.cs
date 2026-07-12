using System.Text.Json.Serialization;

namespace PlaylistShare.Shared;

/// <summary>Стандартный ответ сервера при ошибке.</summary>
public class ErrorResponse
{
    /// <summary>HTTP статус-код.</summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    /// <summary>Сообщение об ошибке.</summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    /// <summary>Дополнительные детали (опционально).</summary>
    [JsonPropertyName("details")]
    public string? Details { get; set; }
}