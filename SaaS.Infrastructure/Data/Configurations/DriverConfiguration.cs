using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Modules.Drivers.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Infrastructure.Data.Configurations
{
    public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(d => d.Phone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.LicenseNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.LicenseClass)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            builder.Property(d => d.Region)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.City)
                .HasMaxLength(100);

            builder.Property(d => d.YearsExperience)
                .IsRequired();

            builder.Property(d => d.Rating)
                .HasColumnType("decimal(3,2)")
                .HasDefaultValue(0m);

            builder.Property(d => d.TripsCompleted)
                .HasDefaultValue(0);

            builder.Property(d => d.Notes)
                .HasMaxLength(2000);

            builder.Property(d => d.TenantId)
                .IsRequired();

            // Unique license number per tenant
            builder.HasIndex(d => new { d.TenantId, d.LicenseNumber }).IsUnique();

            // Fast lookups by status and region
            builder.HasIndex(d => new { d.TenantId, d.Status });
            builder.HasIndex(d => new { d.TenantId, d.Region });
            builder.HasIndex(d => d.TenantId);
        }
    }
}
