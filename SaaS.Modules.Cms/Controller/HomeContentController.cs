using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Cms.Dtos;
using SaaS.Modules.Cms.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SaaS.Modules.Cms.Controller
{
    [ApiController]
    [Route("api/home-content")]
    public sealed class HomeContentController : ControllerBase
    {
        private readonly IHomeContentService _service;
        public HomeContentController(IHomeContentService service) => _service = service;

        // ── Public: frontend reads content ────────────────────────────────────────

        /// <summary>GET /api/home-content?locale=en</summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(HomePageContentDto), 200)]
        public async Task<IActionResult> Get(
            [FromQuery] string locale = "en",
            CancellationToken ct = default)
        {
            var res = await _service.GetAsync(locale, ct);
            return res.IsSuccess ? Ok(res.Value) : NotFound(new { error = res.Error });
        }

        // ── Admin: update page-level fields ──────────────────────────────────────

        /// <summary>PUT /api/home-content?locale=en</summary>
        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(HomePageContentDto), 200)]
        public async Task<IActionResult> Upsert(
            [FromBody] UpdateHomePageContentDto dto,
            [FromQuery] string locale = "en",
            CancellationToken ct = default)
        {
            var res = await _service.UpsertAsync(dto, locale, ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
        }

        /// <summary>POST /api/home-content/hero-image</summary>
        [HttpPost("hero-image")]
        [Authorize(Policy = "AdminOnly")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> UploadHeroImage(
            IFormFile file,
            CancellationToken ct = default)
        {
            var res = await _service.UploadHeroImageAsync(file, ct);
            return res.IsSuccess ? Ok(new { url = res.Value }) : BadRequest(new { error = res.Error });
        }

        // ── Admin: sections ───────────────────────────────────────────────────────

        /// <summary>PUT /api/home-content/sections/{sectionId}</summary>
        [HttpPut("sections/{sectionId:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(HomeSectionDto), 200)]
        public async Task<IActionResult> UpdateSection(
            Guid sectionId,
            [FromBody] UpdateHomeSectionDto dto,
            CancellationToken ct = default)
        {
            var res = await _service.UpdateSectionAsync(sectionId, dto, ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
        }

        /// <summary>POST /api/home-content/sections/{sectionId}/image</summary>
        [HttpPost("sections/{sectionId:guid}/image")]
        [Authorize(Policy = "AdminOnly")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> UploadSectionImage(
            Guid sectionId,
            IFormFile file,
            CancellationToken ct = default)
        {
            var res = await _service.UploadSectionImageAsync(sectionId, file, ct);
            return res.IsSuccess ? Ok(new { url = res.Value }) : BadRequest(new { error = res.Error });
        }

        // ── Admin: blog posts ─────────────────────────────────────────────────────

        /// <summary>POST /api/home-content/blog</summary>
        [HttpPost("blog")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(HomeBlogPostDto), 201)]
        public async Task<IActionResult> CreateBlogPost(
            [FromBody] CreateHomeBlogPostDto dto,
            CancellationToken ct = default)
        {
            var res = await _service.CreateBlogPostAsync(dto, ct);
            return res.IsSuccess ? Created(string.Empty, res.Value) : BadRequest(new { error = res.Error });
        }

        /// <summary>PUT /api/home-content/blog/{postId}</summary>
        [HttpPut("blog/{postId:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(HomeBlogPostDto), 200)]
        public async Task<IActionResult> UpdateBlogPost(
            Guid postId,
            [FromBody] UpdateHomeBlogPostDto dto,
            CancellationToken ct = default)
        {
            var res = await _service.UpdateBlogPostAsync(postId, dto, ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
        }

        /// <summary>DELETE /api/home-content/blog/{postId}</summary>
        [HttpDelete("blog/{postId:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteBlogPost(Guid postId, CancellationToken ct = default)
        {
            var res = await _service.DeleteBlogPostAsync(postId, ct);
            return res.IsSuccess ? Ok(new { message = res.Value }) : BadRequest(new { error = res.Error });
        }

        /// <summary>POST /api/home-content/blog/{postId}/image</summary>
        [HttpPost("blog/{postId:guid}/image")]
        [Authorize(Policy = "AdminOnly")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> UploadBlogImage(
            Guid postId,
            IFormFile file,
            CancellationToken ct = default)
        {
            var res = await _service.UploadBlogImageAsync(postId, file, ct);
            return res.IsSuccess ? Ok(new { url = res.Value }) : BadRequest(new { error = res.Error });
        }
    }
}
