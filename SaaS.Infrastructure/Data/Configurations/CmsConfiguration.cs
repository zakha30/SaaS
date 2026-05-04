using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaaS.Modules.Cms.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Infrastructure.Data.Configurations
{
    public sealed class HomePageContentConfiguration : IEntityTypeConfiguration<HomePageContent>
    {
        public void Configure(EntityTypeBuilder<HomePageContent> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TenantId).IsRequired();
            builder.Property(x => x.Locale).IsRequired().HasMaxLength(10).HasDefaultValue("en");

            builder.Property(x => x.HeroBadge).HasMaxLength(200);
            builder.Property(x => x.HeroTitle).HasMaxLength(300);
            builder.Property(x => x.HeroHighlight).HasMaxLength(200);
            builder.Property(x => x.HeroSubtitle).HasMaxLength(1000);
            builder.Property(x => x.HeroImageUrl).HasMaxLength(1000);
            builder.Property(x => x.HeroCtaPrimary).HasMaxLength(100);
            builder.Property(x => x.HeroCtaSecondary).HasMaxLength(100);

            builder.Property(x => x.WelcomeTitle).HasMaxLength(300);
            builder.Property(x => x.WelcomeSubtitle).HasMaxLength(1000);
            builder.Property(x => x.TrustBarJson).HasMaxLength(2000);

            builder.Property(x => x.BlogHeading).HasMaxLength(300);
            builder.Property(x => x.BlogSubheading).HasMaxLength(500);
            builder.Property(x => x.CtaHeading).HasMaxLength(300);
            builder.Property(x => x.CtaSubheading).HasMaxLength(1000);

            builder.HasMany(x => x.Sections)
                .WithOne(s => s.Content)
                .HasForeignKey(s => s.ContentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BlogPosts)
                .WithOne(b => b.Content)
                .HasForeignKey(b => b.ContentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.TenantId, x.Locale });
        }
    }

    public sealed class HomeSectionConfiguration : IEntityTypeConfiguration<HomeSection>
    {
        public void Configure(EntityTypeBuilder<HomeSection> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TenantId).IsRequired();
            builder.Property(x => x.SectionKey).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Title).HasMaxLength(300);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.ImageUrl).HasMaxLength(1000);
            builder.Property(x => x.StatsJson).HasMaxLength(1000);
            builder.Property(x => x.SortOrder).HasDefaultValue(0);
            builder.Property(x => x.IsVisible).HasDefaultValue(true);

            builder.HasIndex(x => x.ContentId);
            builder.HasIndex(x => new { x.ContentId, x.SectionKey }).IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }

    public sealed class HomeBlogPostConfiguration : IEntityTypeConfiguration<HomeBlogPost>
    {
        public void Configure(EntityTypeBuilder<HomeBlogPost> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TenantId).IsRequired();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(400);
            builder.Property(x => x.Excerpt).HasMaxLength(1000);
            builder.Property(x => x.Author).HasMaxLength(200);
            builder.Property(x => x.Category).HasMaxLength(100);
            builder.Property(x => x.ImageUrl).HasMaxLength(1000);
            builder.Property(x => x.ReadTimeMin).HasDefaultValue(5);
            builder.Property(x => x.SortOrder).HasDefaultValue(0);
            builder.Property(x => x.IsVisible).HasDefaultValue(true);

            builder.HasIndex(x => x.ContentId);
            builder.HasIndex(x => new { x.TenantId, x.IsVisible });
        }
    }
}
