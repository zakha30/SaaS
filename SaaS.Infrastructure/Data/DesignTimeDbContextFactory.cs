using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SaaS.Infrastructure.Services;
using SaaS.Shared;
using System;
using System.IO;

namespace SaaS.Infrastructure.Data;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        // CurrentTenantService starts unresolved (IsResolved = false, CurrentTenantId = Guid.Empty)
        // The global query filters handle this with: !tenantService.IsResolved || ...
        // So migrations see all rows with no tenant filtering applied
        return new AppDbContext(options, new CurrentTenantService());
    }
}
