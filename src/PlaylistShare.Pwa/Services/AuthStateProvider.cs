using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace PlaylistShare.Pwa.Services;

public class AuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly TokenStorage _tokenStorage;
    private readonly ApiClient _apiClient;
    private readonly HttpClient _http;
    private Timer? _refreshTimer;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
    private string? _currentToken;
    private string? _currentRefreshToken;

    public AuthStateProvider(TokenStorage tokenStorage, ApiClient apiClient, HttpClient http)
    {
        _tokenStorage = tokenStorage;
        _apiClient = apiClient;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var (token, refreshToken) = await _tokenStorage.GetTokensAsync();
        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(_currentUser);

        var (isValid, principal) = await ValidateTokenAsync(token);
        if (isValid && principal != null)
        {
            _currentUser = principal;
            _currentToken = token;
            _currentRefreshToken = refreshToken;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            ScheduleTokenRefresh(token, refreshToken);
            return new AuthenticationState(principal);
        }

        // Токен невалиден - пробуем обновить
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var newTokenResponse = await _apiClient.RefreshTokenAsync(refreshToken);
            if (newTokenResponse != null && !string.IsNullOrEmpty(newTokenResponse.Token))
            {
                await MarkUserAsAuthenticated(newTokenResponse.Token, newTokenResponse.RefreshToken);
                // После MarkUserAsAuthenticated состояние обновится через NotifyAuthenticationStateChanged,
                // но текущий вызов всё равно должен вернуть нового пользователя
                var (newIsValid, newPrincipal) = await ValidateTokenAsync(newTokenResponse.Token);
                if (newIsValid && newPrincipal != null)
                    return new AuthenticationState(newPrincipal);
            }
        }

        // Всё плохо - логаут
        await MarkUserAsLoggedOut();
        return new AuthenticationState(_currentUser);
    }

    // Вспомогательный метод проверки валидности токена (включая срок)
    private async Task<(bool IsValid, ClaimsPrincipal? Principal)> ValidateTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Проверяем, не истёк ли токен
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                return (false, null);
            }

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var principal = new ClaimsPrincipal(identity);
            return (true, principal);
        }
        catch
        {
            return (false, null);
        }
    }

    public async Task MarkUserAsAuthenticated(string token, string refreshToken)
    {
        await _tokenStorage.SetTokensAsync(token, refreshToken);
        var principal = ParseToken(token);
        _currentUser = principal ?? new ClaimsPrincipal(new ClaimsIdentity());
        _currentToken = token;
        _currentRefreshToken = refreshToken;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _tokenStorage.ClearTokensAsync();
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        _currentToken = null;
        _currentRefreshToken = null;
        _http.DefaultRequestHeaders.Authorization = null;
        _refreshTimer?.Dispose();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<string?> TryRefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_currentRefreshToken))
            return null;

        try
        {
            var newToken = await _apiClient.RefreshTokenAsync(_currentRefreshToken);
            if (newToken != null && !string.IsNullOrEmpty(newToken.Token))
            {
                await MarkUserAsAuthenticated(newToken.Token, newToken.RefreshToken);
                return newToken.Token;
            }
            else
            {
                await MarkUserAsLoggedOut();
                return null;
            }
        }
        catch
        {
            await MarkUserAsLoggedOut();
            return null;
        }
    }

    public bool IsTokenExpiringSoon()
    {
        if (string.IsNullOrEmpty(_currentToken))
            return false;

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(_currentToken);
        var expiresAt = jwt.ValidTo;
        var timeToExpiry = expiresAt - DateTime.UtcNow;
        return timeToExpiry < TimeSpan.FromMinutes(1);
    }

    private ClaimsPrincipal? ParseToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return null;
        }
    }

    private void ScheduleTokenRefresh(string token, string? refreshToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var expiresAt = jwt.ValidTo;
        var timeToExpiry = expiresAt - DateTime.UtcNow;
        var refreshTime = timeToExpiry - TimeSpan.FromMinutes(5);

        if (refreshTime > TimeSpan.Zero && !string.IsNullOrEmpty(refreshToken))
        {
            _refreshTimer?.Dispose();
            _refreshTimer = new Timer(async _ =>
            {
                await TryRefreshTokenAsync();
            }, null, (int)refreshTime.TotalMilliseconds, Timeout.Infinite);
        }
    }

    public void Dispose() => _refreshTimer?.Dispose();
}