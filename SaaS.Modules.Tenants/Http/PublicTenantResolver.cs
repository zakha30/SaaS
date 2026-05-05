using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Tenants.Http;

/// <summary>
/// Resolves <see cref="ITenantContext"/> for anonymous [AllowAnonymous] API calls using a public tenant slug.
/// Authenticated requests should already have tenant set from the JWT in <c>TenantResolutionMiddleware</c>.
/// </summary>
public static class PublicTenantResolver
{
    public static async Task<IActionResult?> TryResolveForPublicDataAsync(
        ControllerBase c,
        ITenantContext tenant,
        ITenantRepository tenants,
        string? tenantSlug,
        CancellationToken ct)
    {
        if (tenant.IsResolved)
            return null;

        if (c.User.Identity?.IsAuthenticated == true)
        {
            return new ObjectResult(new
            {
                error = "Your session is missing tenant context. Please sign in again."
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        if (string.IsNullOrWhiteSpace(tenantSlug))
        {
            return c.BadRequest(new
            {
                error = "Query parameter 'tenantSlug' is required to browse this organisation's public data."
            });
        }

        var t = await tenants.GetBySlugAsync(tenantSlug.Trim().ToLowerInvariant(), ct);
        if (t is null || !t.IsActive)
        {
            return c.NotFound(new
            {
                error = "Unknown or inactive organisation."
            });
        }

        tenant.SetTenant(t.Id);
        return null;
    }
}
