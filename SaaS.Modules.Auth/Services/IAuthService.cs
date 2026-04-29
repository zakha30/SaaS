using SaaS.Modules.Auth.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Auth.Services;

public interface IAuthService
{
    Task<Result<TokenResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<Result<TokenResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<Result<bool>> RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken ct = default);
}

public interface ITokenService
{
    string GenerateAccessToken(Entities.AppUser user);
    string GenerateRefreshToken();
}