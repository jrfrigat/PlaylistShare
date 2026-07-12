using Microsoft.AspNetCore.DataProtection;
using PlaylistShare.Api.Entities;
using YandexMusic;

namespace PlaylistShare.Api.Services;

/// <summary>
/// Тонкая обёртка над клиентом Яндекс Музыки (<see cref="IYandexMusicClient"/>) для ASP.NET Core.
/// Клиент регистрируется как scoped через <c>AddYandexMusic()</c>, поэтому у каждого запроса своя
/// изолированная сессия. Сервис добавляет авторизацию по сохранённому токену пользователя и
/// шифрование/расшифровку токена для хранения в базе данных.
/// </summary>
public class YandexApiService
{
    private readonly IDataProtector _dataProtector;
    private readonly IYandexMusicClient _client;

    /// <summary>Экземпляр клиента Яндекс Музыки текущего запроса.</summary>
    public IYandexMusicClient Client => _client;

    /// <summary>Признак того, что клиент авторизован (имеет access-токен).</summary>
    public bool IsAuthorized => _client.Authentication.IsAuthenticated;

    public YandexApiService(IDataProtectionProvider provider, IYandexMusicClient client)
    {
        _dataProtector = provider.CreateProtector("YandexTokens");
        _client = client;
    }

    /// <summary>
    /// Авторизует клиент по сохранённому (зашифрованному) токену пользователя.
    /// Возвращает <see langword="null"/>, если токена нет, <see langword="false"/> - если он
    /// повреждён, и <see langword="true"/> при успешной установке токена.
    /// </summary>
    public Task<bool?> AuthAsync(ApplicationUser user)
    {
        if (string.IsNullOrEmpty(user.YandexAccessToken))
            return Task.FromResult<bool?>(null);

        var decryptedToken = DecryptToken(user.YandexAccessToken);
        if (decryptedToken is null)
            return Task.FromResult<bool?>(false);

        _client.Authentication.SignInWithToken(decryptedToken);
        return Task.FromResult<bool?>(true);
    }

    /// <summary>Авторизует клиент напрямую по OAuth-токену.</summary>
    public Task<bool> AuthAsync(string token)
    {
        _client.Authentication.SignInWithToken(token);
        return Task.FromResult(true);
    }

    /// <summary>Зашифровывает токен для хранения в базе данных.</summary>
    public string EncryptToken(string token) => _dataProtector.Protect(token);

    /// <summary>
    /// Расшифровывает токен из базы данных. Если токен повреждён или недействителен, возвращает
    /// <see langword="null"/>.
    /// </summary>
    public string? DecryptToken(string encryptedToken)
    {
        try
        {
            return _dataProtector.Unprotect(encryptedToken);
        }
        catch
        {
            return null;
        }
    }
}
