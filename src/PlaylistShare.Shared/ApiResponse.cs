using System.Text.Json.Serialization;

namespace PlaylistShare.Shared;

/// <summary>Универсальный контейнер ответа API.</summary>
/// <typeparam name="T">Тип данных ответа.</typeparam>
public class ApiResponse<T>
{
    /// <summary>Успешен ли запрос.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>Данные ответа (при успехе).</summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>Сообщение (опционально).</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>Ошибка (при неудаче).</summary>
    [JsonPropertyName("error")]
    public ErrorResponse? Error { get; set; }

    /// <summary>Создаёт успешный ответ.</summary>
    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    /// <summary>Создаёт ответ с ошибкой.</summary>
    public static ApiResponse<T> Fail(ErrorResponse error) =>
        new() { Success = false, Error = error };
}
