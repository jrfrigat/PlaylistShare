using PlaylistShare.Shared;
using PlaylistShare.Shared.SharedPlaylist;
using PlaylistShare.Shared.Yandex;
using System.Net.Http.Json;

namespace PlaylistShare.Pwa.Services.Api;

public class SharedPlaylistClient
{
    private readonly HttpClient _http;

    public SharedPlaylistClient(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// GET /api/sharedplaylist/{token}
    /// </summary>
    public async Task<ApiResponse<SharedPlaylistDto>> GetAsync(string token)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<SharedPlaylistDto>>($"/api/sharedplaylist/{token}");
        return response ?? ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { Message = "Invalid response" });
    }

    /// <summary>
    /// GET /api/sharedplaylist/{token}/tracks
    /// </summary>
    public async Task<ApiResponse<YandexPlaylistData>> GetTracksAsync(string token)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<YandexPlaylistData>>($"/api/sharedplaylist/{token}/tracks");
        return response ?? ApiResponse<YandexPlaylistData>.Fail(new ErrorResponse { Message = "Invalid response" });
    }

    /// <summary>
    /// PUT /api/sharedplaylist/{token}/permissions
    /// </summary>
    public async Task<ApiResponse<SharedPlaylistDto>> UpdatePermissionsAsync(string token, UpdatePermissionsDto permissions)
    {
        var response = await _http.PutAsJsonAsync($"/api/sharedplaylist/{token}/permissions", permissions);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<SharedPlaylistDto>>();
            return result ?? ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse { Message = "Invalid response" });
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ApiResponse<SharedPlaylistDto>>();
            return error ?? ApiResponse<SharedPlaylistDto>.Fail(new ErrorResponse
            {
                StatusCode = (int)response.StatusCode,
                Message = response.ReasonPhrase ?? "Unknown error"
            });
        }
    }

    /// <summary>
    /// POST /api/sharedplaylist/{token}/add-tracks
    /// </summary>
    public async Task<ApiResponse<object>> AddTracksAsync(string token, List<string> trackIds)
    {
        var request = new UpdateTrackListRequest { TrackIds = trackIds };
        var response = await _http.PostAsJsonAsync($"/api/sharedplaylist/{token}/add-tracks", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result ?? ApiResponse<object>.Fail(new ErrorResponse { Message = "Invalid response" });
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return error ?? ApiResponse<object>.Fail(new ErrorResponse
            {
                StatusCode = (int)response.StatusCode,
                Message = response.ReasonPhrase ?? "Unknown error"
            });
        }
    }

    /// <summary>
    /// POST /api/sharedplaylist/{token}/remove-tracks
    /// </summary>
    public async Task<ApiResponse<object>> RemoveTracksAsync(string token, List<string> trackIds)
    {
        var request = new UpdateTrackListRequest { TrackIds = trackIds };
        var response = await _http.PostAsJsonAsync($"/api/sharedplaylist/{token}/remove-tracks", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result ?? ApiResponse<object>.Fail(new ErrorResponse { Message = "Invalid response" });
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return error ?? ApiResponse<object>.Fail(new ErrorResponse
            {
                StatusCode = (int)response.StatusCode,
                Message = response.ReasonPhrase ?? "Unknown error"
            });
        }
    }
}