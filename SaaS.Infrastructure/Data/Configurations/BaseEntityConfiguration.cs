using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Shared;

namespace SaaS.Infrastructure.Data.Configurations;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .HasColumnName("Id")
               .HasColumnType("uniqueidentifier")
               .HasDefaultValueSql("NEWSEQUENTIALID()")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedAt)
               .HasColumnName("CreatedAt")
               .HasColumnType("datetime2")
               .HasDefaultValueSql("SYSUTCDATETIME()")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("UpdatedAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(e => e.IsDeleted)
               .HasColumnName("IsDeleted")
               .HasDefaultValue(false);
    }
}

public abstract class TenantEntityConfiguration<T> : BaseEntityConfiguration<T>
    where T : TenantEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.TenantId)
               .HasColumnName("TenantId")
               .HasColumnType("uniqueidentifier")
               .IsRequired();
    }
}