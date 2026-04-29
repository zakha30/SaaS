using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Modules.Tenants.Entities;

namespace SaaS.Infrastructure.Data.Configurations;

public sealed class TenantConfiguration : BaseEntityConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);

        builder.ToTable("Tenants");

        builder.Property(t => t.Name)
               .HasColumnName("Name")
               .HasColumnType("nvarchar(200)")
               .IsRequired();

        builder.Property(t => t.Slug)
               .HasColumnName("Slug")
               .HasColumnType("nvarchar(100)")
               .IsRequired();

        builder.Property(t => t.ContactEmail)
               .HasColumnName("ContactEmail")
               .HasColumnType("nvarchar(320)")
               .IsRequired();

        builder.Property(t => t.Status)
               .HasColumnName("Status")
               .HasColumnType("nvarchar(20)")
               .IsRequired()
               .HasDefaultValue("Active");

        builder.Property(t => t.PlanId)
               .HasColumnName("PlanId")
               .HasColumnType("uniqueidentifier")
               .IsRequired();

        builder.Property(t => t.PlanExpiresAt)
               .HasColumnName("PlanExpiresAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Ignore(t => t.IsActive);

        builder.HasIndex(t => t.Slug)
               .IsUnique()
               .HasDatabaseName("UIX_Tenants_Slug");

        builder.HasIndex(t => t.Status)
               .HasDatabaseName("IX_Tenants_Status");

        builder.HasIndex(t => t.PlanId)
               .HasDatabaseName("IX_Tenants_PlanId");

        builder.HasMany(t => t.Settings)
               .WithOne()
               .HasForeignKey(s => s.TenantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class TenantSettingsConfiguration : TenantEntityConfiguration<TenantSettings>
{
    public override void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        base.Configure(builder);

        builder.ToTable("TenantSettings");

        builder.Property(s => s.Key)
               .HasColumnName("Key")
               .HasColumnType("nvarchar(100)")
               .IsRequired();

        builder.Property(s => s.Value)
               .HasColumnName("Value")
               .HasColumnType("nvarchar(1000)")
               .IsRequired();
    }
}