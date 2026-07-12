using PlaylistShare.Shared;
using PlaylistShare.Shared.Auth;
using PlaylistShare.Shared.DTO;
using System.Net.Http.Json;

namespace PlaylistShare.Pwa.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http) => _http = http;

    public async Task<LoginResponse?> RefreshTokenAsync(string? refreshToken)
    {
        var response = await _http.PostAsJsonAsync("/api/account/refresh-token", new RefreshTokenRequest { RefreshToken = refreshToken });
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        return apiResponse?.Data;
    }
}