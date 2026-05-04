using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SaaS.Infrastructure.Data;
using SaaS.Infrastructure.Email;
using SaaS.Infrastructure.Identity;
using SaaS.Infrastructure.Modules.Fleet.Services;
using SaaS.Infrastructure.Repositories;
using SaaS.Infrastructure.Repositories.Fleet;
using SaaS.Infrastructure.Services;
using SaaS.Modules.Auth.Entities;
using SaaS.Modules.Auth.Repositories;
using SaaS.Modules.Auth.Services;
using SaaS.Modules.Classifieds.Mappings;
using SaaS.Modules.Classifieds.Repositories;
using SaaS.Modules.Classifieds.Services;
using SaaS.Modules.Dashboard.Services;
using SaaS.Modules.Directory.Mappings;
using SaaS.Modules.Directory.Repositories;
using SaaS.Modules.Directory.Services;
using SaaS.Modules.Drivers.Repositories;
using SaaS.Modules.Drivers.Services;
using SaaS.Modules.Forum.Mappings;
using SaaS.Modules.Forum.Repositories;
using SaaS.Modules.Forum.Services;
using SaaS.Modules.HomePage;
using SaaS.Modules.Jobs.Mappings;
using SaaS.Modules.Jobs.Repositories;
using SaaS.Modules.Jobs.Services;
using SaaS.Modules.Listings.Repositories;
using SaaS.Modules.Listings.Services;
using SaaS.Modules.Loads.Mappings;
// New module namespaces
using SaaS.Modules.Loads.Repositories;
using SaaS.Modules.Loads.Services;
using SaaS.Modules.Notifications.Repositories;
using SaaS.Modules.Notifications.Services;
using SaaS.Modules.Quotes.Repositories;
using SaaS.Modules.Quotes.Services;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Modules.Tenants.Services;
using SaaS.Shared;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static SaaS.Infrastructure.Modules.Fleet.Services.VehicleService;

namespace SaaS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public const string AccessTokenCookie  = JwtSettings.AccessTokenCookie;
    public const string RefreshTokenCookie = JwtSettings.RefreshTokenCookie;

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddScoped<CurrentTenantService>();
        services.AddScoped<ITenantContext>(sp =>
            sp.GetRequiredService<CurrentTenantService>());

        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        var jwtSettings = config
            .GetSection(JwtSettings.Section)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                $"Missing configuration section '{JwtSettings.Section}'.");

        jwtSettings.Validate();

        services.Configure<JwtSettings>(config.GetSection(JwtSettings.Section));
        services.AddScoped<ITokenService, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtSettings.Issuer,
                    ValidAudience            = jwtSettings.Audience,
                    IssuerSigningKey         = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew                = TimeSpan.Zero,
                    RoleClaimType            = ClaimTypes.Role,
                    NameClaimType            = ClaimTypes.NameIdentifier
                };

                opts.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Cookies.TryGetValue(
                                AccessTokenCookie, out var cookieToken)
                            && !string.IsNullOrWhiteSpace(cookieToken))
                        {
                            ctx.Token = cookieToken;
                            return Task.CompletedTask;
                        }

                        var authHeader = ctx.Request.Headers.Authorization.ToString();
                        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            ctx.Token = authHeader["Bearer ".Length..].Trim();

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(opts =>
        {
            opts.AddPolicy("AdminOnly",      p => p.RequireRole(UserRoles.Admin, UserRoles.SuperAdmin));
            opts.AddPolicy("SuperAdminOnly", p => p.RequireRole(UserRoles.SuperAdmin));
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<SaaS.Modules.Listings.Mappings.ListingProfile>();
            cfg.AddProfile<SaaS.Modules.Quotes.Mappings.QuoteProfile>();
            cfg.AddProfile<SaaS.Infrastructure.Modules.Fleet.Mappings.VehicleProfile>();

            cfg.AddProfile<SaaS.Modules.Loads.Mappings.LoadProfile>();
            cfg.AddProfile<SaaS.Modules.Directory.Mappings.DirectoryProfile>();
            cfg.AddProfile<SaaS.Modules.Classifieds.Mappings.ClassifiedProfile>();
            cfg.AddProfile<SaaS.Modules.Jobs.Mappings.JobProfile>();
            cfg.AddProfile<SaaS.Modules.Drivers.Mappings.DriverProfile>();
            cfg.AddProfile<SaaS.Modules.Forum.Mappings.ForumProfile>();

        });

        // Core modules
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<IListingService, ListingService>();

        services.AddScoped<SaaS.Infrastructure.Repositories.Fleet.IVehicleRepository,
                           SaaS.Infrastructure.Repositories.Fleet.VehicleRepository>();
        services.AddScoped<SaaS.Infrastructure.Modules.Fleet.Services.IVehicleService,
                           SaaS.Infrastructure.Modules.Fleet.Services.VehicleService>();

        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IQuoteService, QuoteService>();

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailSender, SendGridEmailSender>();

        // Loads: module interfaces -> Infrastructure implementations
        services.AddScoped<SaaS.Modules.Loads.Repositories.IAvailableLoadRepository,
                           SaaS.Infrastructure.Repositories.AvailableLoadRepository>();
        services.AddScoped<SaaS.Modules.Loads.Services.IAvailableLoadService,
                           SaaS.Modules.Loads.Services.AvailableLoadService>();
        services.AddScoped<SaaS.Modules.Loads.Repositories.IQuoteRequestRepository,
                           SaaS.Infrastructure.Repositories.QuoteRequestRepository>();
        services.AddScoped<SaaS.Modules.Loads.Services.IQuoteRequestService,
                           SaaS.Modules.Loads.Services.QuoteRequestService>();
        services.AddScoped<SaaS.Modules.Loads.Repositories.IQuoteSubmissionRepository,
                           SaaS.Infrastructure.Repositories.QuoteSubmissionRepository>();

        // Directory
        services.AddScoped<SaaS.Modules.Directory.Repositories.IDirectoryRepository,
                           SaaS.Infrastructure.Repositories.DirectoryRepository>();
        services.AddScoped<SaaS.Modules.Directory.Services.IDirectoryService,
                           SaaS.Modules.Directory.Services.DirectoryService>();

        // Classifieds
        services.AddScoped<SaaS.Modules.Classifieds.Repositories.IClassifiedRepository,
                           SaaS.Infrastructure.Repositories.ClassifiedRepository>();
        services.AddScoped<SaaS.Modules.Classifieds.Services.IClassifiedService,
                           SaaS.Modules.Classifieds.Services.ClassifiedService>();

        // Jobs
        services.AddScoped<SaaS.Modules.Jobs.Repositories.IJobRepository,
                           SaaS.Infrastructure.Repositories.JobRepository>();
        services.AddScoped<SaaS.Modules.Jobs.Services.IJobService,
                           SaaS.Modules.Jobs.Services.JobService>();

        // Forum
        services.AddScoped<SaaS.Modules.Forum.Repositories.IThreadRepository,
                           SaaS.Infrastructure.Repositories.ForumRepository>();
        services.AddScoped<SaaS.Modules.Forum.Services.IThreadService,
                           SaaS.Modules.Forum.Services.ThreadService>();

        // Fleet Image Services
        services.AddScoped<IFleetImageRepository, FleetImageRepository>();
        services.AddScoped<IFleetImageService, FleetImageService>();

        services.AddScoped<IDriverRepository,
                      DriverRepository>();
        services.AddScoped<IDriverService,
                           DriverService>();
        services.AddScoped<IHomePageRepository, HomePageRepository>();
        services.AddScoped<HomePageService>();
        services.AddScoped<IHomeContentService, HomeContentService>();

        return services;
    }
}
