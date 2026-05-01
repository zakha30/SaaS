using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using SaaS.Api.Swagger;
using SaaS.Infrastructure.Data;
using SaaS.Infrastructure.Extensions;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Forwarded Headers ──────────────────────────────────────────────────────
// Must be configured before everything else.
// Required behind any reverse proxy (Azure, AWS ALB, Nginx, Cloudflare).
// Without this, HTTPS detection fails → cookies won't be Secure, HSTS won't fire.
builder.Services.Configure<ForwardedHeadersOptions>(opts =>
{
    opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    opts.KnownNetworks.Clear();   // trust all proxies (safe behind a managed LB)
    opts.KnownProxies.Clear();
});

// ── 2. Cookie Policy ──────────────────────────────────────────────────────────
// Enforces SameSite=Lax globally and ensures Secure is set in production.
// SameSite=Lax: cookies are sent on same-site requests and top-level navigations.
// This provides CSRF protection for our auth cookies without breaking the UX.
builder.Services.AddCookiePolicy(opts =>
{
    opts.HttpOnly       = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    opts.Secure         = CookieSecurePolicy.SameAsRequest; // Secure when HTTPS, not forced in dev
    opts.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// ── 3. CORS ───────────────────────────────────────────────────────────────────
// AllowCredentials() is REQUIRED for HttpOnly cookie auth across origins.
// WithOrigins() must list explicit URLs — wildcard (*) is forbidden with credentials.
// Development  → appsettings.Development.json → ["http://localhost:3000"]
// Production   → appsettings.Production.json  → ["https://app.yourdomain.com"]
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(opts =>
    opts.AddPolicy("Frontend", policy =>
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));       // REQUIRED for cookies to be sent cross-origin

// ── 4. Rate Limiting ──────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(opts =>
{
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // "auth" policy — applied via [EnableRateLimiting("auth")] on login/register/refresh
    opts.AddFixedWindowLimiter("auth", limiterOpts =>
    {
        limiterOpts.Window              = TimeSpan.FromMinutes(1);
        limiterOpts.PermitLimit         = 10;
        limiterOpts.QueueLimit          = 0;
        limiterOpts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Global fallback: 300 req / min per IP for all other endpoints
    opts.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window      = TimeSpan.FromMinutes(1),
                PermitLimit = 300,
                QueueLimit  = 0,
            }));

    opts.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.StatusCode      = StatusCodes.Status429TooManyRequests;
        ctx.HttpContext.Response.ContentType     = "application/json";
        await ctx.HttpContext.Response.WriteAsync(
            "{\"error\":\"Too many requests. Please slow down and try again.\"}",
            cancellationToken: ct);
    };
});

// ── 5. Application Services ───────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(SaaS.Modules.Auth.Controllers.AuthController).Assembly)
    .AddApplicationPart(typeof(SaaS.Modules.Tenants.Controllers.TenantsController).Assembly)
    .AddApplicationPart(typeof(SaaS.Modules.Listings.Controllers.ListingsController).Assembly)
    .AddApplicationPart(typeof(SaaS.Modules.Quotes.Controllers.QuotesController).Assembly)
    .AddApplicationPart(typeof(SaaS.Modules.Notifications.Controllers.NotificationsController).Assembly)
    .AddApplicationPart(typeof(SaaS.Modules.Dashboard.Controllers.DashboardController).Assembly)
    .AddApplicationPart(typeof(SaaS.Infrastructure.Data.AppDbContext).Assembly)
    .AddApplicationPart(typeof(SaaS.Modules.Loads.Controllers.AvailableLoadsController).Assembly)
.AddApplicationPart(typeof(SaaS.Modules.Directory.Controllers.DirectoryController).Assembly)
.AddApplicationPart(typeof(SaaS.Modules.Classifieds.Controllers.ClassifiedsController).Assembly)
.AddApplicationPart(typeof(SaaS.Modules.Jobs.Controllers.JobsController).Assembly)
.AddApplicationPart(typeof(SaaS.Modules.Forum.Controllers.ForumController).Assembly);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "SaaS Transport Marketplace API",
        Version     = "v1",
        Description = "Multi-tenant transport marketplace API."
    });

    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Description = "Development only: Enter 'Bearer {token}'. In production, cookies are used automatically.",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat = "JWT"
    });

    opts.OperationFilter<AuthorizeOperationFilter>();
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("sql-server");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
// ── Pipeline ──────────────────────────────────────────────────────────────────
var app = builder.Build();

// Order matters — every middleware comment explains why it sits here.

// 1. Forwarded headers — must be absolute first so the correct scheme/IP
//    flows through to all downstream middleware.
app.UseForwardedHeaders();

// 2. Global exception handler — catch everything before it escapes.
app.UseExceptionHandlerMiddleware();

// 3. Security headers (X-Frame-Options, CSP, Referrer-Policy, etc.)
app.UseSecurityHeaders();

// 4. Cookie policy — applies SameSite and Secure settings globally.
app.UseCookiePolicy();

// 5. CORS — OPTIONS preflight must be handled before HTTPS redirect
//    so the browser receives Access-Control headers before being redirected.
app.UseCors("Frontend");

// 6. HTTPS redirect + HSTS
app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    // Tell browsers to only use HTTPS for 1 year.
    // Only enable once your HTTPS cert is stable.
    app.UseHsts();
}

// 7. Rate limiting — before auth so unauthenticated floods are throttled.
app.UseRateLimiter();

// 8. Swagger — development and staging only, never production.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SaaS Transport API v1");
        c.RoutePrefix = "swagger";
    });
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

// 9. Authentication → Tenant resolution → Authorization (order is critical).
app.UseAuthentication();
app.UseTenantResolution();
app.UseAuthorization();

// 10. Run migrations and seed data on startup.
//await app.ApplyMigrationsAsync();

// 11. Map endpoints.
// Add this in the pipeline after CORS
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = ""
}); // serves wwwroot/
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions { AllowCachingResponses = false });

await app.RunAsync();

public partial class Program { }
