using System.ComponentModel.DataAnnotations;

namespace SaaS.Modules.Auth.DTOs;

// ── Register ──────────────────────────────────────────────────────────────────

public sealed record RegisterDto(
    [Required, MinLength(2), MaxLength(100)] string FirstName,
    [Required, MinLength(2), MaxLength(100)] string LastName,
    [Required, EmailAddress, MaxLength(320)] string Email,
    [Required, MinLength(8), MaxLength(64)] string Password,

    // Tenant info — a new Tenant is created alongside this first user
    [Required, MinLength(2), MaxLength(200)] string TenantName,
    [Required, MinLength(2), MaxLength(100)] string TenantSlug,
    [Required] Guid PlanId);

// ── Login ─────────────────────────────────────────────────────────────────────

public sealed record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password,

    // Tenant slug identifies which tenant this login attempt is for.
    // The same email can exist in multiple tenants.
    [Required] string TenantSlug);

// ── Token responses ───────────────────────────────────────────────────────────

public sealed record TokenResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    UserDto User);

public sealed record UserDto(
    Guid Id,
    Guid TenantId,
    string FullName,
    string Email,
    string Role);

// ── Refresh / Revoke ──────────────────────────────────────────────────────────

public sealed record RefreshTokenDto(
    [Required] string RefreshToken);

// ── Change password ───────────────────────────────────────────────────────────

public sealed record ChangePasswordDto(
    [Required] string CurrentPassword,
    [Required, MinLength(8)] string NewPassword);