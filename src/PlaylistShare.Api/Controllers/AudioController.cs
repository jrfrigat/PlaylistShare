using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlaylistShare.Api.Entities;
using PlaylistShare.Api.Extensions;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared;
using PlaylistShare.Shared.Yandex;
using System.Security.Claims;

namespace PlaylistShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudioController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly YandexMusicService _yandexService;
    private readonly SharedPlaylistService _sharedService;
    private readonly JwtService _jwtService;
    private readonly IHttpClientFactory _httpClientFactory;

    public AudioController(
        UserManager<ApplicationUser> userManager,
        YandexMusicService yandexService,
        SharedPlaylistService sharedService,
        JwtService jwtService,
        IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _yandexService = yandexService;
        _sharedService = sharedService;
        _jwtService = jwtService;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("play-token")]
    [Authorize]
    public IActionResult GetPlayToken()
    {
        var userId = User.GetUserId();
        var token = _jwtService.CreatePlayToken(userId);
        return Ok(ApiResponse<string>.Ok(token));
    }

    [HttpGet("track/{trackId}")]
    [AllowAnonymous]
    public async Task<IActionResult> StreamTrack(string trackId, [FromQuery] string? play_token = null, [FromQuery] string? shared_id = null)
    {
        var user = await GetUserFromPlayToken(play_token);
        if (user == null || user.YandexAccessToken is null) user = await GetUserFromSharedPlaylistId(shared_id);
        if (user == null) return Unauthorized();

        var streamUrl = await _yandexService.GetTrackFileUrlAsync(user, trackId);
        if (string.IsNullOrEmpty(streamUrl)) return NotFound();

        var httpClient = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, streamUrl);

        if (Request.Headers.ContainsKey("Range"))
            request.Headers.Add("Range", Request.Headers["Range"].ToString());

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        Response.StatusCode = (int)response.StatusCode;
        Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "audio/mpeg";

        if (response.Content.Headers.Contains("Content-Range"))
            Response.Headers.Append("Content-Range", response.Content.Headers.ContentRange?.ToString());
        if (response.Headers.Contains("Accept-Ranges"))
            Response.Headers.Append("Accept-Ranges", response.Headers.AcceptRanges?.ToString());
        if (response.Content.Headers.Contains("Content-Length"))
            Response.Headers.Append("Content-Length", response.Content.Headers.ContentLength?.ToString());

        await response.Content.CopyToAsync(Response.Body);
        return new EmptyResult();
    }

    [HttpGet("track-info/{trackId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<YandexTrack>>> GetTrackInfo(string trackId, [FromQuery] string? play_token = null, [FromQuery] string? shared_id = null)
    {
        var user = await GetUserFromPlayToken(play_token);
        if (user == null || user.YandexAccessToken is null) user = await GetUserFromSharedPlaylistId(shared_id);
        if (user == null) return Unauthorized();

        var track = await _yandexService.GetYTrackAsync(user, trackId);
        if (track == null) return NotFound();

        return Ok(ApiResponse<YandexTrack>.Ok(new YandexTrack
        {
            Title = track.Title,
            CoverUri = track.CoverUri ?? string.Empty,
            Artists = track.Artists.Select(a => new YandexArtist
            {
                Id = a.Id.ToString(),
                Name = a.Name,
                CoverUrl = a.Cover.GetUrl(),
                Description = a.Description?.Text ?? string.Empty,
            }).ToList(),
            DurationMs = track.DurationMs,
        }));
    }

    private async Task<ApplicationUser?> GetUserFromPlayToken(string? token)
    {
        if (string.IsNullOrEmpty(token)) return null;
        var userId = _jwtService.ValidatePlayToken(token);
        if (!userId.HasValue) return null;
        return await _userManager.FindByIdAsync(userId.Value.ToString());
    }

    private async Task<ApplicationUser?> GetUserFromSharedPlaylistId(string? sharedId)
    {
        if (string.IsNullOrEmpty(sharedId)) return null;
        var playlist = await _sharedService.GetEntityByTokenAsync(sharedId);
        if (playlist == null) return null;
        if (!_sharedService.CanPlayEveryone(playlist)) return null;
        return await _userManager.FindByIdAsync(playlist.CreatorUserId.ToString());
    }
}
