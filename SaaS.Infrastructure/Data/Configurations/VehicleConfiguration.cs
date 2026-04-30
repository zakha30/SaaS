using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Infrastructure.Modules.Fleet.Entities;

namespace SaaS.Infrastructure.Data.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Make)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Year)
            .IsRequired();

        builder.Property(v => v.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Available");

        builder.Property(v => v.TenantId)
            .IsRequired();
        builder.Property(v => v.TruckType).HasMaxLength(100).IsRequired();
        builder.Property(v => v.TrailerType).HasMaxLength(100);
        builder.Property(v => v.PayloadTons).HasColumnType("decimal(10,2)");
        builder.Property(v => v.Province).HasMaxLength(100).IsRequired();
        builder.Property(v => v.City).HasMaxLength(100);
        builder.Property(v => v.ContactEmail).HasMaxLength(256).IsRequired();
        builder.Property(v => v.ContactPhone).HasMaxLength(50);
        builder.Property(v => v.DailyRate).HasColumnType("decimal(18,2)");
        builder.Property(v => v.Currency).HasMaxLength(3).HasDefaultValue("ZAR");
        builder.Property(v => v.Description).HasMaxLength(2000);
        builder.Property(v => v.ImageUrl).HasMaxLength(500);
        builder.Property(v => v.MembershipTier).HasMaxLength(20).HasDefaultValue("Free");
        builder.Property(v => v.Category).HasConversion<string>().HasMaxLength(20);

        // New indexes
        builder.HasIndex(v => new { v.TenantId, v.Province });
        builder.HasIndex(v => new { v.TenantId, v.TruckType });
        builder.HasIndex(v => new { v.TenantId, v.IsCrossBorderCapable });
        builder.HasIndex(v => v.TenantId);
        builder.HasIndex(v => v.RegistrationNumber).IsUnique();
    }
}

public sealed class FleetImageConfiguration : IEntityTypeConfiguration<FleetImage>
{
    public void Configure(EntityTypeBuilder<FleetImage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.UploadedBy);

        // Foreign key to Vehicle
        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on VehicleId for fast lookups
        builder.HasIndex(x => x.VehicleId);
        // Index on TenantId for multi-tenant queries
       
    }
}
