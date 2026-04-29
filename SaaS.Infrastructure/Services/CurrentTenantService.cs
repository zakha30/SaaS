using SaaS.Shared;
using System;

namespace SaaS.Infrastructure.Services;

// Registered as Scoped — one instance per HTTP request.
// Set once by TenantResolutionMiddleware, then read everywhere via ITenantContext.
public sealed class CurrentTenantService : ITenantContext
{
    private Guid _tenantId;

    // Safe read — returns Guid.Empty when not resolved.
    // Used by EF Core global query filters (evaluated at query time, not model build time).
    public Guid CurrentTenantId => _tenantId;

    // Strict read — throws if not resolved.
    // Used by controllers and services that require an authenticated tenant.
    public Guid TenantId =>
        _tenantId == Guid.Empty
            ? throw new UnauthorizedAccessException(
                "Tenant context has not been resolved. " +
                "Ensure the request carries a valid JWT with a 'tenant_id' claim.")
            : _tenantId;

    // True only after SetTenant has been called with a non-empty Guid
    public bool IsResolved => _tenantId != Guid.Empty;

    public void SetTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException(
                "TenantId cannot be an empty Guid.", nameof(tenantId));

        _tenantId = tenantId;
    }
}