using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Infrastructure.Modules.Fleet.Entities;
using SaaS.Infrastructure.Services;
using SaaS.Modules.Auth.Entities;
using SaaS.Modules.Classifieds.Entities;
using SaaS.Modules.Directory.Entities;
using SaaS.Modules.Forum.Entities;
using SaaS.Modules.Jobs.Entities;
using SaaS.Modules.Listings.Entities;
// New module entity namespaces
using SaaS.Modules.Loads.Entities;
using SaaS.Modules.Notifications.Entities;
using SaaS.Modules.Quotes.Entities;
using SaaS.Modules.Tenants.Entities;
using SaaS.Modules.Tenants.Services;
using SaaS.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    CurrentTenantService tenantService) : DbContext(options)
{
    // ── DbSets ────────────────────────────────────────────────────────────────
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    // Loads
    public DbSet<AvailableLoad> AvailableLoads => Set<AvailableLoad>();
    public DbSet<QuoteRequest> QuoteRequests => Set<QuoteRequest>();
    public DbSet<QuoteSubmission> QuoteSubmissions => Set<QuoteSubmission>();

    // Directory
    public DbSet<BusinessDirectoryEntry> BusinessDirectoryEntries => Set<BusinessDirectoryEntry>();

    // Classifieds
    public DbSet<ClassifiedItem> ClassifiedItems => Set<ClassifiedItem>();

    // Jobs
    public DbSet<JobListing> JobListings => Set<JobListing>();

    // Forum (community-wide)
    public DbSet<ForumThread> ForumThreads => Set<ForumThread>();
    public DbSet<ForumPost> ForumPosts => Set<ForumPost>();
    public DbSet<FleetImage> FleetImages => Set<FleetImage>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all IEntityTypeConfiguration<T> classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ── Global query filters ───────────────────────────────────────────────
        // CRITICAL: Do NOT read tenantService.TenantId here directly.
        // OnModelCreating runs ONCE at startup before any request exists.
        // The lambda is evaluated lazily at query execution time — this is correct.
        // EF Core re-evaluates the closure on every query, picking up the
        // current request's TenantId from the scoped CurrentTenantService.

        // Global tables — soft delete filter only, no TenantId
        modelBuilder.Entity<Tenant>()
            .HasQueryFilter(t => !t.IsDeleted);

        modelBuilder.Entity<Plan>()
            .HasQueryFilter(p => !p.IsDeleted);

        // Tenant-scoped tables — filter by TenantId AND soft delete
        // tenantService.IsResolved check prevents the throw during model build
        modelBuilder.Entity<AppUser>()
            .HasQueryFilter(u =>
                !u.IsDeleted &&
                (!tenantService.IsResolved || u.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<RefreshToken>()
            .HasQueryFilter(rt =>
                !rt.IsDeleted &&
                (!tenantService.IsResolved || rt.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<Listing>()
            .HasQueryFilter(l =>
                !l.IsDeleted &&
                (!tenantService.IsResolved || l.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<Quote>()
            .HasQueryFilter(q =>
                !q.IsDeleted &&
                (!tenantService.IsResolved || q.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<Notification>()
            .HasQueryFilter(n =>
                !n.IsDeleted &&
                (!tenantService.IsResolved || n.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<Vehicle>()
            .HasQueryFilter(v =>
                !v.IsDeleted &&
                (!tenantService.IsResolved || v.TenantId == tenantService.CurrentTenantId));

        // New tenant-scoped entities
        modelBuilder.Entity<AvailableLoad>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!tenantService.IsResolved || e.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<QuoteRequest>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!tenantService.IsResolved || e.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<QuoteSubmission>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!tenantService.IsResolved || e.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<BusinessDirectoryEntry>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!tenantService.IsResolved || e.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<ClassifiedItem>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!tenantService.IsResolved || e.TenantId == tenantService.CurrentTenantId));

        modelBuilder.Entity<JobListing>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!tenantService.IsResolved || e.TenantId == tenantService.CurrentTenantId));

        // Forum entities are community-wide (no tenant filter) — soft-delete only
        modelBuilder.Entity<ForumThread>()
            .HasQueryFilter(t => !t.IsDeleted);

        modelBuilder.Entity<ForumPost>()
            .HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<FleetImage>().HasQueryFilter(x => !x.IsDeleted);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added &&
                entry.Entity.TenantId == Guid.Empty)
            {
                entry.Entity.TenantId = tenantService.CurrentTenantId;
            }

            if (entry.State is EntityState.Modified or EntityState.Deleted)
            {
                if (tenantService.IsResolved &&
                    entry.Entity.TenantId != tenantService.CurrentTenantId)
                {
                    throw new UnauthorizedAccessException(
                        $"Cross-tenant write attempt. " +
                        $"Entity TenantId '{entry.Entity.TenantId}' does not match " +
                        $"current tenant '{tenantService.CurrentTenantId}'.");
                }
            }
        }

        foreach (var entry in ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(ct);
    }
}