using SaaS.Modules.Auth.Entities;

namespace SaaS.Modules.Auth.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AppUser?> GetByEmailAndTenantAsync(string email, Guid tenantId, CancellationToken ct = default);
    Task<bool> EmailExistsInTenantAsync(string email, Guid tenantId, CancellationToken ct = default);
    Task AddAsync(AppUser user, CancellationToken ct = default);
    Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}