using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaylistShare.Database.Entities;
using PlaylistShare.Api.Services;
using PlaylistShare.Shared;
using PlaylistShare.Shared.Auth;
using PlaylistShare.Shared.DTO;

namespace PlaylistShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtService _jwtService;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<LoginResponse>.Fail(new ErrorResponse
            {
                StatusCode = 400,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            }));

        return await GenerateTokenResponse(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return Unauthorized(ApiResponse<LoginResponse>.Fail(new ErrorResponse { StatusCode = 401, Message = "Неверное имя пользователя или пароль" }));

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(ApiResponse<LoginResponse>.Fail(new ErrorResponse { StatusCode = 401, Message = "Неверное имя пользователя или пароль" }));

        return await GenerateTokenResponse(user);
    }

    private async Task<ActionResult<ApiResponse<LoginResponse>>> GenerateTokenResponse(ApplicationUser user)
    {
        var (token, refreshToken, expiration) = await _jwtService.GenerateTokenAsync(user);
        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = expiration
        }));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        // Без AsNoTracking: JwtService выдаёт этому же user новый RefreshToken и сохраняет его.
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && u.RefreshTokenExpiryUtc > DateTime.UtcNow, cancellationToken);
        if (user == null)
            return Unauthorized(ApiResponse<LoginResponse>.Fail(new ErrorResponse { StatusCode = 401, Message = "Неверный или просроченный refresh token" }));

        return await GenerateTokenResponse(user);
    }
}