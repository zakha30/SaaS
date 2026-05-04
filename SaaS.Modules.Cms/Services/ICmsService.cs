using Microsoft.AspNetCore.Http;
using SaaS.Modules.Cms.Dtos;
using SaaS.Modules.Cms.Entities;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SaaS.Modules.Cms.Services
{
    public interface IHomeContentService
    {
        // Page-level
        Task<Result<HomePageContentDto>> GetAsync(string locale = "en", CancellationToken ct = default);
        Task<Result<HomePageContentDto>> UpsertAsync(UpdateHomePageContentDto dto, string locale = "en", CancellationToken ct = default);
        Task<Result<string>> UploadHeroImageAsync(IFormFile file, CancellationToken ct = default);

        // Sections
        Task<Result<HomeSectionDto>> UpdateSectionAsync(Guid sectionId, UpdateHomeSectionDto dto, CancellationToken ct = default);
        Task<Result<string>> UploadSectionImageAsync(Guid sectionId, IFormFile file, CancellationToken ct = default);

        // Blog posts
        Task<Result<HomeBlogPostDto>> CreateBlogPostAsync(CreateHomeBlogPostDto dto, CancellationToken ct = default);
        Task<Result<HomeBlogPostDto>> UpdateBlogPostAsync(Guid postId, UpdateHomeBlogPostDto dto, CancellationToken ct = default);
        Task<Result<string>> DeleteBlogPostAsync(Guid postId, CancellationToken ct = default);
        Task<Result<string>> UploadBlogImageAsync(Guid postId, IFormFile file, CancellationToken ct = default);
    }

    // ── Implementation ────────────────────────────────────────────────────────────

    
}
