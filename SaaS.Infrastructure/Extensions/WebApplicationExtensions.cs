using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaaS.Infrastructure.Data;
using SaaS.Infrastructure.Middleware;
using System.Linq;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
        if (pending.Count > 0)
        {
            logger.LogInformation("Applying {Count} pending migration(s)...", pending.Count);
            await db.Database.MigrateAsync();
            logger.LogInformation("Migrations applied.");
        }

        await DatabaseSeeder.SeedAsync(db, logger);
    }

    public static WebApplication UseExceptionHandlerMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    public static WebApplication UseTenantResolution(this WebApplication app)
    {
        app.UseMiddleware<TenantResolutionMiddleware>();
        return app;
    }

    /// <summary>
    /// Adds production-grade HTTP security headers (X-Frame-Options, CSP, etc.)
    /// Call this early in the pipeline, before CORS and auth.
    /// </summary>
    public static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
        return app;
    }
}
