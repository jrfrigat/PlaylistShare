using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlaylistShare.Api.Entities;
using PlaylistShare.Api.Extensions;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared;
using PlaylistShare.Shared.Profile;
using PlaylistShare.Shared.Yandex;

namespace PlaylistShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class YandexAccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly YandexAuthService _yandexService;

    public YandexAccountController(UserManager<ApplicationUser> userManager, YandexAuthService yandexService)
    {
        _userManager = userManager;
        _yandexService = yandexService;
    }

    [HttpPost("token")]
    public async Task<ActionResult<ApiResponse<object>>> SetToken([FromBody] SetYandexTokenRequest request)
    {
        var userId = User.GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Unauthorized();

        await SaveYandexTokenAsync(user, request.Token);
        return Ok(ApiResponse<object>.Ok(new { message = "Токен сохранён" }));
    }

    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse<YandexTokenStatus>>> GetStatus()
    {
        var userId = User.GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Unauthorized();

        var hasToken = !string.IsNullOrEmpty(user.YandexAccessToken);
        var isValid = hasToken && user.YandexTokenExpiryUtc > DateTime.UtcNow;

        return Ok(ApiResponse<YandexTokenStatus>.Ok(new YandexTokenStatus
        {
            HasToken = hasToken,
            IsValid = isValid,
            ExpiryUtc = user.YandexTokenExpiryUtc
        }));
    }

    [HttpGet("qr")]
    public async Task<ActionResult<ApiResponse<YandexAuthQr>>> GetQr()
    {
        var userId = User.GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Unauthorized();

        var qr = await _yandexService.GetQrOrGenerate(user);
        return Ok(ApiResponse<YandexAuthQr>.Ok(qr));
    }

    [HttpGet("qr/{sessionId}")]
    public async Task<IActionResult> CheckQr(int sessionId)
    {
        var userId = User.GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Unauthorized();

        var checkResult = await _yandexService.CheckQrAsync(sessionId);
        if (checkResult == null) return NotFound();

        if (checkResult.Status == Shared.Enums.YandexAuthQrStatus.Authorized && _yandexService.AuthorizedAccessToken is { } token)
        {
            await SaveYandexTokenAsync(user, token);
        }

        return Ok(ApiResponse<YandexAuthQrCheck>.Ok(checkResult));
    }

    private async Task SaveYandexTokenAsync(ApplicationUser user, string token)
    {
        user.YandexAccessToken = _yandexService.Service.EncryptToken(token);
        user.YandexTokenExpiryUtc = DateTime.UtcNow.AddMonths(1);
        await _userManager.UpdateAsync(user);
    }
}
