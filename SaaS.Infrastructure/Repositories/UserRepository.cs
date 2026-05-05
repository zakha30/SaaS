using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Auth.Entities;
using SaaS.Modules.Auth.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<IReadOnlyList<AppUser>> GetAllInTenantAsync(CancellationToken ct = default) =>
        await db.Users
            .AsNoTracking()
            .OrderBy(u => u.Email)
            .ToListAsync(ct);

    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Users
                .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<AppUser?> GetByEmailAndTenantAsync(
        string email, Guid tenantId, CancellationToken ct = default) =>
        // IgnoreQueryFilters because login runs before tenant context is resolved
        await db.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    u => u.Email == email.ToLowerInvariant()
                      && u.TenantId == tenantId
                      && !u.IsDeleted, ct);

    public async Task<bool> EmailExistsInTenantAsync(
        string email, Guid tenantId, CancellationToken ct = default) =>
        await db.Users
                .IgnoreQueryFilters()
                .AnyAsync(
                    u => u.Email == email.ToLowerInvariant()
                      && u.TenantId == tenantId
                      && !u.IsDeleted, ct);

    public async Task AddAsync(AppUser user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);

    public async Task AddRefreshTokenAsync(
        RefreshToken token, CancellationToken ct = default) =>
        await db.RefreshTokens.AddAsync(token, ct);

    public async Task<RefreshToken?> GetRefreshTokenAsync(
        string token, CancellationToken ct = default) =>
        await db.RefreshTokens
                .IgnoreQueryFilters()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsDeleted, ct);

    public async Task RevokeAllUserTokensAsync(
        Guid userId, CancellationToken ct = default)
    {
        var tokens = await db.RefreshTokens
                             .IgnoreQueryFilters()
                             .Where(rt => rt.UserId == userId && rt.IsActive())
                             .ToListAsync(ct);

        foreach (var t in tokens)
        {
            t.IsRevoked = true;
            t.RevokedAt = DateTime.UtcNow;
        }
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}

// Extension so IsActive() can be used in LINQ-to-Objects
file static class RefreshTokenExtensions
{
    public static bool IsActive(this RefreshToken token) =>
        !token.IsRevoked && DateTime.UtcNow < token.ExpiresAt;
}