using Microsoft.AspNetCore.Identity;
using SaaS.Modules.Auth.DTOs;
using SaaS.Modules.Auth.Entities;
using SaaS.Modules.Auth.Repositories;
using SaaS.Modules.Tenants.Entities;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Auth.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    ITenantRepository tenantRepository,
    ITokenService tokenService,
    IPasswordHasher<AppUser> passwordHasher) : IAuthService
{
    public async Task<Result<TokenResponseDto>> RegisterAsync(
        RegisterDto dto, CancellationToken ct = default)
    {
        // ── 1. Validate slug uniqueness ───────────────────────────────────────
        if (await tenantRepository.SlugExistsAsync(dto.TenantSlug, ct))
            return Result<TokenResponseDto>.Failure(
                $"Tenant slug '{dto.TenantSlug}' is already taken.");

        // ── 2. Create tenant ──────────────────────────────────────────────────
        var tenant = new Tenant
        {
            Name = dto.TenantName.Trim(),
            Slug = dto.TenantSlug.ToLowerInvariant().Trim(),
            ContactEmail = dto.Email.ToLowerInvariant().Trim(),
            PlanId = dto.PlanId,
            Status = TenantStatus.Active
        };

        await tenantRepository.AddAsync(tenant, ct);
        await tenantRepository.SaveChangesAsync(ct);

        // ── 3. Validate email uniqueness within the new tenant ────────────────
        if (await userRepository.EmailExistsInTenantAsync(dto.Email, tenant.Id, ct))
            return Result<TokenResponseDto>.Failure(
                $"Email '{dto.Email}' is already registered in this tenant.");

        // ── 4. Create the first user as Admin of the new tenant ───────────────
        var user = new AppUser
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.ToLowerInvariant().Trim(),
            Role = UserRoles.Admin,   // first user is always Admin
            TenantId = tenant.Id
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

        await userRepository.AddAsync(user, ct);
        await userRepository.SaveChangesAsync(ct);

        // ── 5. Issue tokens ───────────────────────────────────────────────────
        return Result<TokenResponseDto>.Success(
            await IssueTokensAsync(user, ct));
    }

    public async Task<Result<TokenResponseDto>> LoginAsync(
        LoginDto dto, CancellationToken ct = default)
    {
        // ── 1. Resolve tenant by slug ─────────────────────────────────────────
        var tenant = await tenantRepository.GetBySlugAsync(dto.TenantSlug, ct);
        if (tenant is null)
            return Result<TokenResponseDto>.Failure("Invalid credentials.");

        if (!tenant.IsActive)
            return Result<TokenResponseDto>.Failure(
                "This account has been suspended. Please contact support.");

        // ── 2. Find user by email within the tenant ───────────────────────────
        var user = await userRepository.GetByEmailAndTenantAsync(
            dto.Email, tenant.Id, ct);

        if (user is null)
            return Result<TokenResponseDto>.Failure("Invalid credentials.");

        // ── 3. Verify password ────────────────────────────────────────────────
        var verification = passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, dto.Password);

        if (verification == PasswordVerificationResult.Failed)
            return Result<TokenResponseDto>.Failure("Invalid credentials.");

        // Rehash if the password was hashed with an older algorithm
        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
        }

        // ── 4. Update last login ──────────────────────────────────────────────
        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.SaveChangesAsync(ct);

        // ── 5. Issue tokens ───────────────────────────────────────────────────
        return Result<TokenResponseDto>.Success(
            await IssueTokensAsync(user, ct));
    }

    public async Task<Result<TokenResponseDto>> RefreshTokenAsync(
        string refreshToken, CancellationToken ct = default)
    {
        var token = await userRepository.GetRefreshTokenAsync(refreshToken, ct);

        if (token is null)
            return Result<TokenResponseDto>.Failure("Refresh token not found.");

        if (!token.IsActive)
            return Result<TokenResponseDto>.Failure(
                token.IsRevoked ? "Refresh token has been revoked."
                                : "Refresh token has expired.");

        // Rotate — revoke old token and issue new one
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await userRepository.SaveChangesAsync(ct);

        return Result<TokenResponseDto>.Success(
            await IssueTokensAsync(token.User, ct));
    }

    public async Task<Result<bool>> RevokeTokenAsync(
        string refreshToken, CancellationToken ct = default)
    {
        var token = await userRepository.GetRefreshTokenAsync(refreshToken, ct);

        if (token is null)
            return Result<bool>.Failure("Refresh token not found.");

        if (!token.IsActive)
            return Result<bool>.Failure("Token is already revoked or expired.");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await userRepository.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ChangePasswordAsync(
        Guid userId, ChangePasswordDto dto, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result<bool>.Failure("User not found.");

        // Verify current password before allowing change
        var verification = passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, dto.CurrentPassword);

        if (verification == PasswordVerificationResult.Failed)
            return Result<bool>.Failure("Current password is incorrect.");

        user.PasswordHash = passwordHasher.HashPassword(user, dto.NewPassword);

        // Revoke all existing refresh tokens — force re-login on all devices
        await userRepository.RevokeAllUserTokensAsync(userId, ct);
        await userRepository.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task<TokenResponseDto> IssueTokensAsync(
        AppUser user, CancellationToken ct)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var rawRefresh = tokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        var refreshToken = new RefreshToken
        {
            Token = rawRefresh,
            UserId = user.Id,
            TenantId = user.TenantId,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await userRepository.AddRefreshTokenAsync(refreshToken, ct);
        await userRepository.SaveChangesAsync(ct);

        return new TokenResponseDto(
            AccessToken: accessToken,
            RefreshToken: rawRefresh,
            AccessTokenExpiresAt: expiresAt,
            User: new UserDto(
                user.Id,
                user.TenantId,
                user.FullName,
                user.Email,
                user.Role));
    }
}