using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SaaS.Modules.Auth.DTOs;
using SaaS.Modules.Auth.Services;

namespace SaaS.Modules.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthService authService,
    IConfiguration config) : ControllerBase
{
    private const string AccessTokenCookie = "transhub_access";
    private const string RefreshTokenCookie = "transhub_refresh";

    private void SetAuthCookies(string accessToken, string refreshToken)
    {
        var expiryMinutes = config.GetValue<int>("JwtSettings:ExpiryMinutes", 60);
        var refreshExpiryDays = config.GetValue<int>("JwtSettings:RefreshExpiryDays", 30);
        var isHttps = HttpContext.Request.IsHttps;

        Response.Cookies.Append(AccessTokenCookie, accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes),
            Path = "/",
        });

        Response.Cookies.Append(RefreshTokenCookie, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/api/auth",
            Expires = DateTimeOffset.UtcNow.AddDays(refreshExpiryDays),
        });
    }

    private void ClearAuthCookies()
    {
        var isHttps = HttpContext.Request.IsHttps;

        Response.Cookies.Append(AccessTokenCookie, string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UnixEpoch,
            Path = "/",
        });

        Response.Cookies.Append(RefreshTokenCookie, string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UnixEpoch,
            Path = "/api/auth",
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto dto, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(dto, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        SetAuthCookies(result.Value!.AccessToken, result.Value.RefreshToken);

        return StatusCode(201, new
        {
            user = result.Value.User,
            accessTokenExpiresAt = result.Value.AccessTokenExpiresAt,
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await authService.LoginAsync(dto, ct);
        if (!result.IsSuccess)
            return StatusCode(401, new { error = result.Error });

        SetAuthCookies(result.Value!.AccessToken, result.Value.RefreshToken);

        return Ok(new
        {
            user = result.Value.User,
            accessTokenExpiresAt = result.Value.AccessTokenExpiresAt,
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookie, out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
        {
            return StatusCode(401, new { error = "Refresh token cookie missing." });
        }

        var result = await authService.RefreshTokenAsync(refreshToken, ct);
        if (!result.IsSuccess)
        {
            ClearAuthCookies();
            return StatusCode(401, new { error = result.Error });
        }

        SetAuthCookies(result.Value!.AccessToken, result.Value.RefreshToken);

        return Ok(new
        {
            user = result.Value.User,
            accessTokenExpiresAt = result.Value.AccessTokenExpiresAt,
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (Request.Cookies.TryGetValue(RefreshTokenCookie, out var refreshToken)
            && !string.IsNullOrWhiteSpace(refreshToken))
        {
            await authService.RevokeTokenAsync(refreshToken, ct);
        }

        ClearAuthCookies();
        return NoContent();
    }

    [HttpPatch("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto, CancellationToken ct)
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException());

        var result = await authService.ChangePasswordAsync(userId, dto, ct);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        ClearAuthCookies();
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok(new
    {
        userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
        tenantId = User.FindFirst("tenant_id")?.Value,
        email = User.FindFirst(ClaimTypes.Email)?.Value,
        role = User.FindFirst(ClaimTypes.Role)?.Value,
        fullName = User.FindFirst("full_name")?.Value,
    });
}