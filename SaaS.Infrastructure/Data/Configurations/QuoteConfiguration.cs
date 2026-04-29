using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Modules.Quotes.Entities;

namespace SaaS.Infrastructure.Data.Configurations;

public sealed class QuoteConfiguration : TenantEntityConfiguration<Quote>
{
    public override void Configure(EntityTypeBuilder<Quote> builder)
    {
        base.Configure(builder);

        builder.ToTable("Quotes");

        builder.Property(q => q.ListingId)
               .HasColumnName("ListingId")
               .HasColumnType("uniqueidentifier")
               .IsRequired();

        builder.Property(q => q.TransporterId)
               .HasColumnName("TransporterId")
               .HasColumnType("uniqueidentifier")
               .IsRequired();

        builder.Property(q => q.Price)
               .HasColumnName("Price")
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(q => q.Currency)
               .HasColumnName("Currency")
               .HasColumnType("nchar(3)")
               .IsRequired();

        builder.Property(q => q.Message)
               .HasColumnName("Message")
               .HasColumnType("nvarchar(1000)")
               .IsRequired();

        builder.Property(q => q.Status)
               .HasColumnName("Status")
               .HasConversion<string>()
               .HasColumnType("nvarchar(20)")
               .IsRequired();

        builder.Property(q => q.RejectionReason)
               .HasColumnName("RejectionReason")
               .HasColumnType("nvarchar(500)")
               .IsRequired(false);

        builder.Property(q => q.AcceptedAt)
               .HasColumnName("AcceptedAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(q => q.RejectedAt)
               .HasColumnName("RejectedAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(q => q.WithdrawnAt)
               .HasColumnName("WithdrawnAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(q => q.ValidUntil)
               .HasColumnName("ValidUntil")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.HasIndex(q => q.TenantId)
               .HasDatabaseName("IX_Quotes_TenantId");

        builder.HasIndex(q => new { q.TenantId, q.ListingId })
               .HasDatabaseName("IX_Quotes_TenantId_ListingId");

        builder.HasIndex(q => new { q.TenantId, q.TransporterId })
               .HasDatabaseName("IX_Quotes_TenantId_TransporterId");

        builder.HasIndex(q => new { q.TenantId, q.ListingId, q.TransporterId, q.Status })
               .HasDatabaseName("IX_Quotes_Duplicate_Check");

        builder.HasIndex(q => q.ValidUntil)
               .HasDatabaseName("IX_Quotes_ValidUntil")
               .HasFilter("[Status] = 'Pending' AND [IsDeleted] = 0");
    }
}