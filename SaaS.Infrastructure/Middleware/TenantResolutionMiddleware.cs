using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SaaS.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Middleware;

// Placed AFTER UseAuthentication() in the pipeline so ClaimsPrincipal is populated.
// Placed BEFORE UseAuthorization() so tenant context is available to all handlers.
public sealed class TenantResolutionMiddleware(
    RequestDelegate next,
    ILogger<TenantResolutionMiddleware> logger)
{
    // Paths that do not require a resolved tenant context.
    // Checked with StartsWith — order does not matter.
    private static readonly HashSet<string> _excludedPrefixes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/refresh",
            "/api/plans",
            "/health",
            "/swagger",
            "/favicon.ico"
        };

    public async Task InvokeAsync(
        HttpContext context, CurrentTenantService tenantService)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Root path — nothing to resolve
        if (path == "/")
        {
            await next(context);
            return;
        }

        // Skip tenant resolution for public / infrastructure paths
        if (_excludedPrefixes.Any(prefix =>
                path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        {
            await next(context);
            return;
        }

        // Authenticated users: always attach tenant from JWT (even on [AllowAnonymous] endpoints)
        // so CMS + listings respect the logged-in tenant without requiring ?tenantSlug=.
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var jwtTenant = context.User.FindFirst("tenant_id");
            if (jwtTenant != null &&
                Guid.TryParse(jwtTenant.Value, out var tid) &&
                tid != Guid.Empty)
            {
                tenantService.SetTenant(tid);
            }
        }

        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

        if (allowAnonymous)
        {
            await next(context);
            return;
        }

        // Protected endpoints require a resolved tenant (anonymous requests must use tenantSlug in-controller).
        if (!tenantService.IsResolved)
        {
            logger.LogWarning(
                "Request to {Path} has no tenant context (login required or tenantSlug for public routes).", path);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"error\":\"Tenant could not be resolved. Sign in or pass tenantSlug for public content.\"}");
            return;
        }

        logger.LogDebug(
            "Tenant resolved: {TenantId} for {Path}", tenantService.CurrentTenantId, path);

        await next(context);
    }
}