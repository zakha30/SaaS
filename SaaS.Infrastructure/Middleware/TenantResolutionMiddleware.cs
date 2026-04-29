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

        // ⭐ NEW: Check AllowAnonymous
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

        if (allowAnonymous)
        {
            await next(context);
            return;
        }

        // Attempt to resolve tenant from JWT claim
        var claim = context.User.FindFirst("tenant_id");

        if (claim is null ||
            !Guid.TryParse(claim.Value, out var tenantId) ||
            tenantId == Guid.Empty)
        {
            logger.LogWarning(
                "Request to {Path} is missing a valid 'tenant_id' claim.", path);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"error\":\"Tenant could not be resolved from the provided token.\"}");
            return;
        }

        tenantService.SetTenant(tenantId);

        logger.LogDebug(
            "Tenant resolved: {TenantId} for {Path}", tenantId, path);

        await next(context);
    }
}