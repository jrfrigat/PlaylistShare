using Microsoft.JSInterop;

namespace PlaylistShare.Pwa.Services;

public class TokenStorage
{
    private readonly IJSRuntime _js;
    private const string TokenKey = "jwt_token";
    private const string RefreshTokenKey = "refresh_token";

    public TokenStorage(IJSRuntime js) => _js = js;

    public async Task SetTokensAsync(string token, string refreshToken)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        await _js.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, refreshToken);
    }

    public async Task<(string? token, string? refreshToken)> GetTokensAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", TokenKey);
        var refreshToken = await _js.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);
        return (token, refreshToken);
    }

    public async Task ClearTokensAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await _js.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
    }
}
