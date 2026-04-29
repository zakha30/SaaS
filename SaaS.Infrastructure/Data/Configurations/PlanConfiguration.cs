using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Modules.Tenants.Entities;

namespace SaaS.Infrastructure.Data.Configurations;

public sealed class PlanConfiguration : BaseEntityConfiguration<Plan>
{
    public override void Configure(EntityTypeBuilder<Plan> builder)
    {
        base.Configure(builder);

        builder.ToTable("Plans");

        builder.Property(p => p.Name)
               .HasColumnName("Name")
               .HasColumnType("nvarchar(100)")
               .IsRequired();

        builder.Property(p => p.Tier)
               .HasColumnName("Tier")
               .HasColumnType("nvarchar(50)")
               .IsRequired();

        builder.Property(p => p.MonthlyPrice)
               .HasColumnName("MonthlyPrice")
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(p => p.MaxUsers)
               .HasColumnName("MaxUsers")
               .IsRequired();

        builder.Property(p => p.MaxListings)
               .HasColumnName("MaxListings")
               .IsRequired();

        builder.Property(p => p.IsActive)
               .HasColumnName("IsActive")
               .HasDefaultValue(true);

        builder.HasIndex(p => p.Name)
               .IsUnique()
               .HasDatabaseName("UIX_Plans_Name");
    }
}