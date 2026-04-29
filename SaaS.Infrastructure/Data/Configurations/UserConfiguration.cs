using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Modules.Auth.Entities;

namespace SaaS.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : TenantEntityConfiguration<AppUser>
{
    public override void Configure(EntityTypeBuilder<AppUser> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.Property(u => u.FirstName)
               .HasColumnName("FirstName")
               .HasColumnType("nvarchar(100)")
               .IsRequired();

        builder.Property(u => u.LastName)
               .HasColumnName("LastName")
               .HasColumnType("nvarchar(100)")
               .IsRequired();

        builder.Property(u => u.Email)
               .HasColumnName("Email")
               .HasColumnType("nvarchar(320)")
               .IsRequired();

        builder.Property(u => u.PasswordHash)
               .HasColumnName("PasswordHash")
               .HasColumnType("nvarchar(512)")
               .IsRequired();

        builder.Property(u => u.Role)
               .HasColumnName("Role")
               .HasColumnType("nvarchar(20)")
               .IsRequired();

        builder.Property(u => u.EmailVerified)
               .HasColumnName("EmailVerified")
               .HasDefaultValue(false);

        builder.Property(u => u.LastLoginAt)
               .HasColumnName("LastLoginAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Ignore(u => u.FullName);

        builder.HasIndex(u => new { u.TenantId, u.Email })
               .IsUnique()
               .HasDatabaseName("UIX_Users_TenantId_Email");

        builder.HasIndex(u => u.TenantId)
               .HasDatabaseName("IX_Users_TenantId");

        builder.HasMany(u => u.RefreshTokens)
               .WithOne(rt => rt.User)
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class RefreshTokenConfiguration : TenantEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("RefreshTokens");

        builder.Property(rt => rt.Token)
               .HasColumnName("Token")
               .HasColumnType("nvarchar(512)")
               .IsRequired();

        builder.Property(rt => rt.UserId)
               .HasColumnName("UserId")
               .HasColumnType("uniqueidentifier")
               .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
               .HasColumnName("ExpiresAt")
               .HasColumnType("datetime2")
               .IsRequired();

        builder.Property(rt => rt.IsRevoked)
               .HasColumnName("IsRevoked")
               .HasDefaultValue(false);

        builder.Property(rt => rt.RevokedAt)
               .HasColumnName("RevokedAt")
               .HasColumnType("datetime2")
               .IsRequired(false);

        builder.Property(rt => rt.ReplacedByToken)
               .HasColumnName("ReplacedByToken")
               .HasColumnType("nvarchar(512)")
               .IsRequired(false);

        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsActive);

        builder.HasIndex(rt => rt.Token)
               .IsUnique()
               .HasDatabaseName("UIX_RefreshTokens_Token");

        builder.HasIndex(rt => new { rt.TenantId, rt.UserId })
               .HasDatabaseName("IX_RefreshTokens_TenantId_UserId");
    }
}