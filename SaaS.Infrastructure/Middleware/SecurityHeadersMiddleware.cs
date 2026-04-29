using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Middleware;

/// <summary>
/// Adds production-grade HTTP security headers to every response.
/// Registered in Program.cs before auth middleware.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        // Prevent MIME-type sniffing — browser must respect Content-Type
        headers["X-Content-Type-Options"] = "nosniff";

        // Prevent this site from being embedded in iframes (clickjacking protection)
        headers["X-Frame-Options"] = "DENY";

        // Disable browser XSS auditor (deprecated but still sent for old browsers)
        headers["X-XSS-Protection"] = "0";

        // Only send the origin (no path) as Referer when navigating to other sites
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Restrict which browser features are allowed
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";

        // Content Security Policy
        // Adjust script-src / connect-src to match your CDNs and API domains in production.
        headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline'; " +   // unsafe-inline needed for Swagger UI
            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
            "font-src 'self' https://fonts.gstatic.com; " +
            "img-src 'self' data:; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none';";

        await next(context);
    }
}
