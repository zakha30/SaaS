using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Cms.Dtos;
using SaaS.Modules.Cms.Entities;
using SaaS.Modules.Cms.Services;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Repositories
{
    public sealed class HomeContentService(AppDbContext db) : IHomeContentService
    {
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        // ── Helpers ───────────────────────────────────────────────────────────────

        private IQueryable<HomePageContent> ContentQuery =>
            db.Set<HomePageContent>()
              .Include(c => c.Sections.Where(s => !s.IsDeleted).OrderBy(s => s.SortOrder))
              .Include(c => c.BlogPosts.Where(b => !b.IsDeleted && b.IsVisible).OrderBy(b => b.SortOrder));

        private static List<StatItemDto> ParseStats(string? json) =>
            string.IsNullOrWhiteSpace(json)
                ? new()
                : JsonSerializer.Deserialize<List<StatItemDto>>(json, _json) ?? new();

        private static List<StatItemDto> ParseTrust(string? json) => ParseStats(json);

        private static HomePageContentDto MapDto(HomePageContent c) => new()
        {
            Id = c.Id,
            Locale = c.Locale,
            HeroBadge = c.HeroBadge,
            HeroTitle = c.HeroTitle,
            HeroHighlight = c.HeroHighlight,
            HeroSubtitle = c.HeroSubtitle,
            HeroImageUrl = c.HeroImageUrl,
            HeroCtaPrimary = c.HeroCtaPrimary,
            HeroCtaSecondary = c.HeroCtaSecondary,
            WelcomeTitle = c.WelcomeTitle,
            WelcomeSubtitle = c.WelcomeSubtitle,
            TrustBar = ParseTrust(c.TrustBarJson),
            BlogHeading = c.BlogHeading,
            BlogSubheading = c.BlogSubheading,
            CtaHeading = c.CtaHeading,
            CtaSubheading = c.CtaSubheading,
            Sections = c.Sections.Select(s => new HomeSectionDto
            {
                Id = s.Id,
                SectionKey = s.SectionKey,
                Title = s.Title,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                SortOrder = s.SortOrder,
                IsVisible = s.IsVisible,
                Stats = ParseStats(s.StatsJson),
            }).ToList(),
            BlogPosts = c.BlogPosts.Select(b => new HomeBlogPostDto
            {
                Id = b.Id,
                Title = b.Title,
                Excerpt = b.Excerpt,
                Author = b.Author,
                Category = b.Category,
                ReadTimeMin = b.ReadTimeMin,
                ImageUrl = b.ImageUrl,
                PublishedAt = b.PublishedAt,
                SortOrder = b.SortOrder,
                IsVisible = b.IsVisible,
            }).ToList(),
        };

        private static async Task<string> SaveFileAsync(
            IFormFile file, string subfolder, CancellationToken ct)
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", subfolder);
            Directory.CreateDirectory(dir);
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(dir, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await file.CopyToAsync(stream, ct);
            return $"/uploads/{subfolder}/{fileName}";
        }

        private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };
        private static Result<T>? ValidateImage<T>(IFormFile file)
        {
            if (file.Length == 0) return Result<T>.Failure("No file provided.");
            if (file.Length > 5 * 1024 * 1024) return Result<T>.Failure("File exceeds 5MB limit.");
            if (!AllowedImageTypes.Contains(file.ContentType.ToLowerInvariant()))
                return Result<T>.Failure("Only JPG, PNG, WEBP, and GIF are allowed.");
            return null;
        }

        // ── Page ──────────────────────────────────────────────────────────────────

        public async Task<Result<HomePageContentDto>> GetAsync(string locale = "en", CancellationToken ct = default)
        {
            var content = await ContentQuery
                .FirstOrDefaultAsync(c => c.Locale == locale && !c.IsDeleted, ct);

            if (content is null)
            {
                // Fall back to 'en' if requested locale not found
                content = await ContentQuery
                    .FirstOrDefaultAsync(c => c.Locale == "en" && !c.IsDeleted, ct);
            }

            return content is null
                ? Result<HomePageContentDto>.Failure("Home content not found. Run the DB seed script.")
                : Result<HomePageContentDto>.Success(MapDto(content));
        }

        public async Task<Result<HomePageContentDto>> UpsertAsync(
            UpdateHomePageContentDto dto, string locale = "en", CancellationToken ct = default)
        {
            var content = await db.Set<HomePageContent>()
                .Include(c => c.Sections)
                .Include(c => c.BlogPosts)
                .FirstOrDefaultAsync(c => c.Locale == locale && !c.IsDeleted, ct);

            if (content is null)
            {
                // Auto-create if missing (first-time setup)
                content = new HomePageContent { Locale = locale };
                db.Set<HomePageContent>().Add(content);
            }

            // Apply non-null fields from dto
            if (dto.HeroBadge is not null) content.HeroBadge = dto.HeroBadge;
            if (dto.HeroTitle is not null) content.HeroTitle = dto.HeroTitle;
            if (dto.HeroHighlight is not null) content.HeroHighlight = dto.HeroHighlight;
            if (dto.HeroSubtitle is not null) content.HeroSubtitle = dto.HeroSubtitle;
            if (dto.HeroImageUrl is not null) content.HeroImageUrl = dto.HeroImageUrl;
            if (dto.HeroCtaPrimary is not null) content.HeroCtaPrimary = dto.HeroCtaPrimary;
            if (dto.HeroCtaSecondary is not null) content.HeroCtaSecondary = dto.HeroCtaSecondary;
            if (dto.WelcomeTitle is not null) content.WelcomeTitle = dto.WelcomeTitle;
            if (dto.WelcomeSubtitle is not null) content.WelcomeSubtitle = dto.WelcomeSubtitle;
            if (dto.BlogHeading is not null) content.BlogHeading = dto.BlogHeading;
            if (dto.BlogSubheading is not null) content.BlogSubheading = dto.BlogSubheading;
            if (dto.CtaHeading is not null) content.CtaHeading = dto.CtaHeading;
            if (dto.CtaSubheading is not null) content.CtaSubheading = dto.CtaSubheading;
            if (dto.TrustBar is not null)
                content.TrustBarJson = JsonSerializer.Serialize(dto.TrustBar);

            content.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);

            return Result<HomePageContentDto>.Success(MapDto(content));
        }

        public async Task<Result<string>> UploadHeroImageAsync(IFormFile file, CancellationToken ct = default)
        {
            var err = ValidateImage<string>(file);
            if (err is not null) return err;

            try
            {
                var url = await SaveFileAsync(file, "cms/hero", ct);

                // Persist the URL to the 'en' content record
                var content = await db.Set<HomePageContent>()
                    .FirstOrDefaultAsync(c => c.Locale == "en" && !c.IsDeleted, ct);
                if (content is not null)
                {
                    content.HeroImageUrl = url;
                    content.UpdatedAt = DateTime.UtcNow;
                    await db.SaveChangesAsync(ct);
                }

                return Result<string>.Success(url);
            }
            catch (Exception ex) { return Result<string>.Failure($"Upload failed: {ex.Message}"); }
        }

        // ── Sections ──────────────────────────────────────────────────────────────

        public async Task<Result<HomeSectionDto>> UpdateSectionAsync(
            Guid sectionId, UpdateHomeSectionDto dto, CancellationToken ct = default)
        {
            var section = await db.Set<HomeSection>()
                .FirstOrDefaultAsync(s => s.Id == sectionId && !s.IsDeleted, ct);
            if (section is null) return Result<HomeSectionDto>.Failure("Section not found.");

            if (dto.Title is not null) section.Title = dto.Title;
            if (dto.Description is not null) section.Description = dto.Description;
            if (dto.ImageUrl is not null) section.ImageUrl = dto.ImageUrl;
            if (dto.SortOrder is not null) section.SortOrder = dto.SortOrder.Value;
            if (dto.IsVisible is not null) section.IsVisible = dto.IsVisible.Value;
            if (dto.Stats is not null) section.StatsJson = JsonSerializer.Serialize(dto.Stats);

            section.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);

            return Result<HomeSectionDto>.Success(new HomeSectionDto
            {
                Id = section.Id,
                SectionKey = section.SectionKey,
                Title = section.Title,
                Description = section.Description,
                ImageUrl = section.ImageUrl,
                SortOrder = section.SortOrder,
                IsVisible = section.IsVisible,
                Stats = ParseStats(section.StatsJson),
            });
        }

        public async Task<Result<string>> UploadSectionImageAsync(
            Guid sectionId, IFormFile file, CancellationToken ct = default)
        {
            var err = ValidateImage<string>(file);
            if (err is not null) return err;

            var section = await db.Set<HomeSection>()
                .FirstOrDefaultAsync(s => s.Id == sectionId && !s.IsDeleted, ct);
            if (section is null) return Result<string>.Failure("Section not found.");

            try
            {
                var url = await SaveFileAsync(file, "cms/sections", ct);
                section.ImageUrl = url;
                section.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
                return Result<string>.Success(url);
            }
            catch (Exception ex) { return Result<string>.Failure($"Upload failed: {ex.Message}"); }
        }

        // ── Blog posts ────────────────────────────────────────────────────────────

        public async Task<Result<HomeBlogPostDto>> CreateBlogPostAsync(
            CreateHomeBlogPostDto dto, CancellationToken ct = default)
        {
            var content = await db.Set<HomePageContent>()
                .FirstOrDefaultAsync(c => c.Locale == "en" && !c.IsDeleted, ct);
            if (content is null) return Result<HomeBlogPostDto>.Failure("Home content not initialised.");

            var post = new HomeBlogPost
            {
                ContentId = content.Id,
                Title = dto.Title,
                Excerpt = dto.Excerpt,
                Author = dto.Author,
                Category = dto.Category,
                ReadTimeMin = dto.ReadTimeMin,
                ImageUrl = dto.ImageUrl,
                PublishedAt = dto.PublishedAt ?? DateTime.UtcNow,
                SortOrder = dto.SortOrder,
                IsVisible = dto.IsVisible,
            };

            db.Set<HomeBlogPost>().Add(post);
            await db.SaveChangesAsync(ct);

            return Result<HomeBlogPostDto>.Success(MapPostDto(post));
        }

        public async Task<Result<HomeBlogPostDto>> UpdateBlogPostAsync(
            Guid postId, UpdateHomeBlogPostDto dto, CancellationToken ct = default)
        {
            var post = await db.Set<HomeBlogPost>()
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted, ct);
            if (post is null) return Result<HomeBlogPostDto>.Failure("Blog post not found.");

            if (dto.Title is not null) post.Title = dto.Title;
            if (dto.Excerpt is not null) post.Excerpt = dto.Excerpt;
            if (dto.Author is not null) post.Author = dto.Author;
            if (dto.Category is not null) post.Category = dto.Category;
            if (dto.ReadTimeMin is not null) post.ReadTimeMin = dto.ReadTimeMin.Value;
            if (dto.ImageUrl is not null) post.ImageUrl = dto.ImageUrl;
            if (dto.PublishedAt is not null) post.PublishedAt = dto.PublishedAt.Value;
            if (dto.SortOrder is not null) post.SortOrder = dto.SortOrder.Value;
            if (dto.IsVisible is not null) post.IsVisible = dto.IsVisible.Value;

            post.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);

            return Result<HomeBlogPostDto>.Success(MapPostDto(post));
        }

        public async Task<Result<string>> DeleteBlogPostAsync(Guid postId, CancellationToken ct = default)
        {
            var post = await db.Set<HomeBlogPost>()
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted, ct);
            if (post is null) return Result<string>.Failure("Blog post not found.");

            post.IsDeleted = true;
            post.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return Result<string>.Success("Blog post deleted.");
        }

        public async Task<Result<string>> UploadBlogImageAsync(
            Guid postId, IFormFile file, CancellationToken ct = default)
        {
            var err = ValidateImage<string>(file);
            if (err is not null) return err;

            var post = await db.Set<HomeBlogPost>()
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted, ct);
            if (post is null) return Result<string>.Failure("Blog post not found.");

            try
            {
                var url = await SaveFileAsync(file, "cms/blog", ct);
                post.ImageUrl = url;
                post.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
                return Result<string>.Success(url);
            }
            catch (Exception ex) { return Result<string>.Failure($"Upload failed: {ex.Message}"); }
        }

        private static HomeBlogPostDto MapPostDto(HomeBlogPost b) => new()
        {
            Id = b.Id,
            Title = b.Title,
            Excerpt = b.Excerpt,
            Author = b.Author,
            Category = b.Category,
            ReadTimeMin = b.ReadTimeMin,
            ImageUrl = b.ImageUrl,
            PublishedAt = b.PublishedAt,
            SortOrder = b.SortOrder,
            IsVisible = b.IsVisible,
        };
    }
}
