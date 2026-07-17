using System.ComponentModel.DataAnnotations;

namespace PlaylistShare.Shared.Profile;

/// <summary>Запрос на привязку аккаунта Яндекс.Музыки по access токену.</summary>
public class SetYandexTokenRequest
{
    /// <summary>Access токен Яндекс.Музыки.</summary>
    // Только Required: токен ложится в AspNetUsers.YandexAccessToken (nvarchar(max)) уже
    // зашифрованным, то есть длины колонки, от которой можно оттолкнуться, в схеме нет.
    [Required]
    public string Token { get; set; } = string.Empty;
}
