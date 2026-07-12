using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PlaylistShare.Api.Data;
using PlaylistShare.Api.Entities;
using PlaylistShare.Shared.Enums;
using PlaylistShare.Shared.Yandex;
using YandexMusic;
using YandexMusic.Authentication;

namespace PlaylistShare.Api.Services;

/// <summary>
/// Вход в Яндекс "по QR" через официальный OAuth device-code flow клиента YandexMusic.
///
/// Раньше использовался Passport magic-link (StartQrSignInAsync), но Яндекс закрыл его капчей
/// (GET passport.yandex.ru/auth отвечает 302), из-за чего эндпоинт /qr падал с 500. Device-code flow
/// (RequestDeviceCodeAsync + PollDeviceTokenAsync) официальный и без капчи: пользователь открывает
/// VerificationUrl (ya.ru/device), вводит короткий UserCode, а сервер опрашивает статус. Незавершённая
/// попытка (клиент + DeviceCode) живёт в памяти процесса (IMemoryCache) её короткое время жизни; строка
/// YandexAuthSession в БД хранит только ссылку и таймстемпы. После подтверждения клиент отдаёт OAuth-токен,
/// который сохраняется (зашифрованным) у пользователя - дальнейшие запросы авторизуются именно им.
/// </summary>
public class YandexAuthService
{
    private static readonly TimeSpan AttemptLifetime = TimeSpan.FromMinutes(5);

    private readonly YandexApiService _apiService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;

    /// <summary>Сервис-обёртка клиента (используется контроллером для шифрования токена).</summary>
    public YandexApiService Service => _apiService;

    /// <summary>
    /// OAuth-токен, полученный при последнем успешном подтверждении. Контроллер сохраняет его
    /// пользователю. Имеет смысл только сразу после <see cref="CheckQrAsync"/> со статусом
    /// <see cref="YandexAuthQrStatus.Authorized"/>.
    /// </summary>
    public string? AuthorizedAccessToken { get; private set; }

    public YandexAuthService(YandexApiService apiService, ApplicationDbContext dbContext, IMemoryCache cache)
    {
        _apiService = apiService;
        _dbContext = dbContext;
        _cache = cache;
    }

    /// <summary>Возвращает действующую попытку входа пользователя или создаёт новую.</summary>
    internal async Task<YandexAuthQr> GetQrOrGenerate(ApplicationUser user)
    {
        var cutoff = DateTime.UtcNow.Subtract(AttemptLifetime);
        var recent = await _dbContext.YandexAuthSessions
            .Where(s => s.UserId == user.Id && !s.IsConfirmed && s.CreatedAt > cutoff)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        // Переиспользуем существующую попытку, только если она ещё жива в кэше памяти.
        foreach (var session in recent)
        {
            if (_cache.TryGetValue(CacheKey(session.Id), out DeviceAttempt? attempt) && attempt is not null)
            {
                return BuildQr(session.QrCodeUrl, attempt.Code.UserCode, session.Id);
            }
        }

        return await GenerateQrAsync(user);
    }

    /// <summary>Создаёт новую попытку: запрашивает device-code у OAuth Яндекса и сохраняет её состояние.</summary>
    internal async Task<YandexAuthQr> GenerateQrAsync(ApplicationUser user)
    {
        var client = new YandexMusicClient();
        DeviceCode code;
        try
        {
            code = await client.Authentication.RequestDeviceCodeAsync();
        }
        catch
        {
            client.Dispose();
            throw new Exception("Не удалось получить код для входа в Яндекс.");
        }

        var session = new YandexAuthSession
        {
            UserId = user.Id,
            QrCodeUrl = code.VerificationUrl,
            CreatedAt = DateTime.UtcNow,
            IsConfirmed = false,
        };

        _dbContext.YandexAuthSessions.Add(session);
        await _dbContext.SaveChangesAsync();

        CacheAttempt(session.Id, new DeviceAttempt(client, code));

        return BuildQr(code.VerificationUrl, code.UserCode, session.Id);
    }

    /// <summary>Опрашивает статус попытки один раз.</summary>
    internal async Task<YandexAuthQrCheck?> CheckQrAsync(int sessionId)
    {
        var session = await _dbContext.YandexAuthSessions.FindAsync(sessionId);
        if (session is null)
            return null;

        if (!_cache.TryGetValue(CacheKey(sessionId), out DeviceAttempt? attempt) || attempt is null)
            return new YandexAuthQrCheck { Status = YandexAuthQrStatus.Expired };

        OAuthToken? token;
        try
        {
            token = await attempt.Client.Authentication.PollDeviceTokenAsync(attempt.Code.Code);
        }
        catch
        {
            // Код истёк, отклонён или другая ошибка OAuth - попытку дальше опрашивать нет смысла.
            _cache.Remove(CacheKey(sessionId));
            return new YandexAuthQrCheck { Status = YandexAuthQrStatus.Error };
        }

        if (token is null)
            return new YandexAuthQrCheck { Status = YandexAuthQrStatus.Pending };

        if (string.IsNullOrEmpty(token.AccessToken))
            return new YandexAuthQrCheck { Status = YandexAuthQrStatus.Error };

        AuthorizedAccessToken = token.AccessToken;

        session.IsConfirmed = true;
        session.ConfirmedAt = DateTime.UtcNow;
        await _dbContext.YandexAuthSessions
            .Where(t => t.UserId == session.UserId)
            .ExecuteDeleteAsync();

        _cache.Remove(CacheKey(sessionId)); // disposes the cached client via the eviction callback

        return new YandexAuthQrCheck { Status = YandexAuthQrStatus.Authorized };
    }

    /// <summary>Собирает DTO попытки: ссылку подтверждения, короткий код и QR этой ссылки.</summary>
    private static YandexAuthQr BuildQr(string verificationUrl, string userCode, int sessionId) => new()
    {
        QrLink = verificationUrl,
        UserCode = userCode,
        SessionId = sessionId.ToString(),
    };

    private void CacheAttempt(int sessionId, DeviceAttempt attempt)
    {
        var options = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = AttemptLifetime }
            .RegisterPostEvictionCallback(static (_, value, _, _) => (value as IDisposable)?.Dispose());
        _cache.Set(CacheKey(sessionId), attempt, options);
    }

    private static string CacheKey(int sessionId) => $"yandex-qr:{sessionId}";

    /// <summary>An in-progress device-code sign-in held in memory for its short lifetime.</summary>
    private sealed class DeviceAttempt(YandexMusicClient client, DeviceCode code) : IDisposable
    {
        public YandexMusicClient Client { get; } = client;
        public DeviceCode Code { get; } = code;
        public void Dispose() => Client.Dispose();
    }
}
