using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaaS.Modules.Tenants.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db, ILogger logger)
    {
        // Only seed if Plans table is empty
        if (await db.Plans.AnyAsync())
            return;

        logger.LogInformation("Seeding default plans...");

        var plans = new List<Plan>
        {
            new()
            {
                Id           = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name         = "Starter",
                Tier         = "Starter",
                MonthlyPrice = 0m,
                MaxUsers     = 5,
                MaxListings  = 10,
                IsActive     = true
            },
            new()
            {
                Id           = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name         = "Pro",
                Tier         = "Pro",
                MonthlyPrice = 49.99m,
                MaxUsers     = 25,
                MaxListings  = 100,
                IsActive     = true
            },
            new()
            {
                Id           = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name         = "Enterprise",
                Tier         = "Enterprise",
                MonthlyPrice = 199.99m,
                MaxUsers     = 999,
                MaxListings  = 9999,
                IsActive     = true
            }
        };

        await db.Plans.AddRangeAsync(plans);
        await db.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} plans.", plans.Count);
    }
}