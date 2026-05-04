using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Modules.Cms.Entities
{
    public sealed class HomePageContent : TenantEntity
    {
        // Hero
        public string? HeroBadge { get; set; }
        public string? HeroTitle { get; set; }
        public string? HeroHighlight { get; set; }
        public string? HeroSubtitle { get; set; }
        public string? HeroImageUrl { get; set; }
        public string? HeroCtaPrimary { get; set; }
        public string? HeroCtaSecondary { get; set; }

        // Welcome
        public string? WelcomeTitle { get; set; }
        public string? WelcomeSubtitle { get; set; }

        // Trust bar — serialized JSON array
        public string? TrustBarJson { get; set; }

        // Blog
        public string? BlogHeading { get; set; }
        public string? BlogSubheading { get; set; }

        // CTA
        public string? CtaHeading { get; set; }
        public string? CtaSubheading { get; set; }

        // Locale  ('en' | 'ar')
        public string Locale { get; set; } = "en";

        // Navigation
        public ICollection<HomeSection> Sections { get; set; } = new List<HomeSection>();
        public ICollection<HomeBlogPost> BlogPosts { get; set; } = new List<HomeBlogPost>();
    }

    public sealed class HomeSection : TenantEntity
    {
        public Guid ContentId { get; set; }
        public string SectionKey { get; set; } = string.Empty; // 'fleet' | 'drivers' …
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;

        // Stats JSON: [{ "label":"...", "value":"..." }]
        public string? StatsJson { get; set; }

        public HomePageContent? Content { get; set; }
    }

    public sealed class HomeBlogPost : TenantEntity
    {
        public Guid ContentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public int ReadTimeMin { get; set; } = 5;
        public string? ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;

        public HomePageContent? Content { get; set; }
    }
}
