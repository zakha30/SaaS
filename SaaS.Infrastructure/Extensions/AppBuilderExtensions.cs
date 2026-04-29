using Microsoft.AspNetCore.Builder;
using SaaS.Infrastructure.Middleware;

namespace SaaS.Infrastructure.Extensions;

/// <summary>
/// Application builder extension methods for middleware registration.
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    /// Registers the SecurityHeadersMiddleware in the pipeline.
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
