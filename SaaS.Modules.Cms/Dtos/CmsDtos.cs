using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SaaS.Modules.Cms.Dtos
{
    public sealed class StatItemDto
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    // ── Section ───────────────────────────────────────────────────────────────────

    public sealed class HomeSectionDto
    {
        public Guid Id { get; set; }
        public string SectionKey { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; }
        public List<StatItemDto> Stats { get; set; } = new();
    }

    public sealed class UpdateHomeSectionDto
    {
        [MaxLength(300)] public string? Title { get; set; }
        [MaxLength(1000)] public string? Description { get; set; }
        [MaxLength(1000)] public string? ImageUrl { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsVisible { get; set; }
        public List<StatItemDto>? Stats { get; set; }
    }

    // ── Blog post ─────────────────────────────────────────────────────────────────

    public sealed class HomeBlogPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public int ReadTimeMin { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; }
    }

    public sealed class CreateHomeBlogPostDto
    {
        [Required, MaxLength(400)] public string Title { get; set; } = string.Empty;
        [MaxLength(1000)] public string? Excerpt { get; set; }
        [MaxLength(200)] public string? Author { get; set; }
        [MaxLength(100)] public string? Category { get; set; }
        [Range(1, 120)] public int ReadTimeMin { get; set; } = 5;
        [MaxLength(1000)] public string? ImageUrl { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;
    }

    public sealed class UpdateHomeBlogPostDto
    {
        [MaxLength(400)] public string? Title { get; set; }
        [MaxLength(1000)] public string? Excerpt { get; set; }
        [MaxLength(200)] public string? Author { get; set; }
        [MaxLength(100)] public string? Category { get; set; }
        [Range(1, 120)] public int? ReadTimeMin { get; set; }
        [MaxLength(1000)] public string? ImageUrl { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsVisible { get; set; }
    }

    // ── Page content ──────────────────────────────────────────────────────────────

    public sealed class HomePageContentDto
    {
        public Guid Id { get; set; }
        public string Locale { get; set; } = "en";

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

        // Trust bar
        public List<StatItemDto> TrustBar { get; set; } = new();

        // Blog
        public string? BlogHeading { get; set; }
        public string? BlogSubheading { get; set; }

        // CTA
        public string? CtaHeading { get; set; }
        public string? CtaSubheading { get; set; }

        // Children
        public List<HomeSectionDto> Sections { get; set; } = new();
        public List<HomeBlogPostDto> BlogPosts { get; set; } = new();
    }

    public sealed class UpdateHomePageContentDto
    {
        [MaxLength(200)] public string? HeroBadge { get; set; }
        [MaxLength(300)] public string? HeroTitle { get; set; }
        [MaxLength(200)] public string? HeroHighlight { get; set; }
        [MaxLength(1000)] public string? HeroSubtitle { get; set; }
        [MaxLength(1000), Url] public string? HeroImageUrl { get; set; }
        [MaxLength(100)] public string? HeroCtaPrimary { get; set; }
        [MaxLength(100)] public string? HeroCtaSecondary { get; set; }

        [MaxLength(300)] public string? WelcomeTitle { get; set; }
        [MaxLength(1000)] public string? WelcomeSubtitle { get; set; }

        public List<StatItemDto>? TrustBar { get; set; }

        [MaxLength(300)] public string? BlogHeading { get; set; }
        [MaxLength(500)] public string? BlogSubheading { get; set; }

        [MaxLength(300)] public string? CtaHeading { get; set; }
        [MaxLength(1000)] public string? CtaSubheading { get; set; }
    }
}
