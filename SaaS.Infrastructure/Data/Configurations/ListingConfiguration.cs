using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Modules.Listings.Entities;

namespace SaaS.Infrastructure.Data.Configurations;

public sealed class ListingConfiguration : TenantEntityConfiguration<Listing>
{
    public override void Configure(EntityTypeBuilder<Listing> builder)
    {
        base.Configure(builder);

        builder.ToTable("Listings");

        builder.Property(l => l.Title)
               .HasColumnName("Title")
               .HasColumnType("nvarchar(300)")
               .IsRequired();

        builder.Property(l => l.Description)
               .HasColumnName("Description")
               .HasColumnType("nvarchar(2000)")
               .IsRequired(false);

        builder.Property(l => l.Type)
               .HasColumnName("Type")
               .HasConversion<string>()
               .HasColumnType("nvarchar(20)")
               .IsRequired();

        builder.Property(l => l.LocationFrom)
               .HasColumnName("LocationFrom")
               .HasColumnType("nvarchar(150)")
               .IsRequired();

        builder.Property(l => l.LocationTo)
               .HasColumnName("LocationTo")
               .HasColumnType("nvarchar(150)")
               .IsRequired();

        builder.Property(l => l.Price)
               .HasColumnName("Price")
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(l => l.Currency)
               .HasColumnName("Currency")
               .HasColumnType("nchar(3)")
               .IsRequired();

        builder.Property(l => l.Status)
               .HasColumnName("Status")
               .HasConversion<string>()
               .HasColumnType("nvarchar(20)")
               .IsRequired();

        builder.Property(l => l.UserId)
               .HasColumnName("UserId")
               .HasColumnType("uniqueidentifier")
               .IsRequired();

        builder.Property(l => l.WeightKg)
               .HasColumnName("WeightKg")
               .HasColumnType("decimal(10,2)")
               .IsRequired(false);

        builder.Property(l => l.VolumeM3)
               .HasColumnName("VolumeM3")
               .HasColumnType("decimal(10,3)")
               .IsRequired(false);

        builder.Property(l => l.CargoType)
               .HasColumnName("CargoType")
               .HasColumnType("nvarchar(100)")
               .IsRequired(false);

        builder.Property(l => l.AvailableFrom)
               .HasColumnName("AvailableFrom")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(l => l.ExpiresAt)
               .HasColumnName("ExpiresAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.HasIndex(l => l.TenantId)
               .HasDatabaseName("IX_Listings_TenantId");

        builder.HasIndex(l => new { l.TenantId, l.Status })
               .HasDatabaseName("IX_Listings_TenantId_Status");

        builder.HasIndex(l => new { l.TenantId, l.UserId })
               .HasDatabaseName("IX_Listings_TenantId_UserId");

        builder.HasIndex(l => new { l.TenantId, l.LocationFrom, l.LocationTo })
               .HasDatabaseName("IX_Listings_Route")
               .HasFilter("[Status] = 'Active' AND [IsDeleted] = 0");

        builder.HasIndex(l => new { l.TenantId, l.Type })
               .HasDatabaseName("IX_Listings_TenantId_Type")
               .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(l => new { l.TenantId, l.Price })
               .HasDatabaseName("IX_Listings_TenantId_Price")
               .HasFilter("[Status] = 'Active' AND [IsDeleted] = 0");

        builder.HasIndex(l => new { l.TenantId, l.CreatedAt })
               .HasDatabaseName("IX_Listings_TenantId_CreatedAt")
               .HasFilter("[IsDeleted] = 0");
    }
}