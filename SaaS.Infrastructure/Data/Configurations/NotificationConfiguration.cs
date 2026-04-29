using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Modules.Notifications.Entities;

namespace SaaS.Infrastructure.Data.Configurations;

public sealed class NotificationConfiguration : TenantEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);

        builder.ToTable("Notifications");

        builder.Property(n => n.RecipientUserId)
               .HasColumnName("RecipientUserId")
               .HasColumnType("uniqueidentifier")
               .IsRequired();

        builder.Property(n => n.RelatedQuoteId)
               .HasColumnName("RelatedQuoteId")
               .HasColumnType("uniqueidentifier")
               .IsRequired(false);

        builder.Property(n => n.RelatedListingId)
               .HasColumnName("RelatedListingId")
               .HasColumnType("uniqueidentifier")
               .IsRequired(false);

        builder.Property(n => n.Subject)
               .HasColumnName("Subject")
               .HasColumnType("nvarchar(300)")
               .IsRequired();

        builder.Property(n => n.Body)
               .HasColumnName("Body")
               .HasColumnType("nvarchar(max)")
               .IsRequired();

        builder.Property(n => n.Type)
               .HasColumnName("Type")
               .HasConversion<string>()
               .HasColumnType("nvarchar(20)")
               .IsRequired();

        builder.Property(n => n.IsRead)
               .HasColumnName("IsRead")
               .HasDefaultValue(false);

        builder.Property(n => n.IsSent)
               .HasColumnName("IsSent")
               .HasDefaultValue(false);

        builder.Property(n => n.SentAt)
               .HasColumnName("SentAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(n => n.ReadAt)
               .HasColumnName("ReadAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.HasIndex(n => n.TenantId)
               .HasDatabaseName("IX_Notifications_TenantId");

        builder.HasIndex(n => new { n.TenantId, n.RecipientUserId })
               .HasDatabaseName("IX_Notifications_TenantId_RecipientUserId");

        builder.HasIndex(n => new { n.TenantId, n.RecipientUserId, n.IsRead })
               .HasDatabaseName("IX_Notifications_Unread")
               .HasFilter("[IsDeleted] = 0 AND [IsRead] = 0");
    }
}