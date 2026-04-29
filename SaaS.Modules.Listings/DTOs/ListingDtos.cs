using System.ComponentModel.DataAnnotations;
using SaaS.Modules.Listings.Entities;

namespace SaaS.Modules.Listings.DTOs;

// ── Create ────────────────────────────────────────────────────────────────────

public sealed record CreateListingDto(
    [Required, MinLength(3), MaxLength(300)] string Title,
    [MaxLength(2000)] string? Description,
    [Required] ListingType Type,
    [Required, MaxLength(150)] string LocationFrom,
    [Required, MaxLength(150)] string LocationTo,
    [Required, Range(0, double.MaxValue)] decimal Price,
    [MaxLength(3)] string Currency = "USD",
                                              decimal? WeightKg = null,
                                              decimal? VolumeM3 = null,
    [MaxLength(100)] string? CargoType = null,
                                              DateTime? AvailableFrom = null,
                                              DateTime? ExpiresAt = null);

// ── Update ────────────────────────────────────────────────────────────────────

public sealed record UpdateListingDto(
    [MinLength(3), MaxLength(300)] string? Title,
    [MaxLength(2000)] string? Description,
                                    ListingType? Type,
    [MaxLength(150)] string? LocationFrom,
    [MaxLength(150)] string? LocationTo,
    [Range(0, double.MaxValue)] decimal? Price,
    [MaxLength(3)] string? Currency,
                                    decimal? WeightKg,
                                    decimal? VolumeM3,
    [MaxLength(100)] string? CargoType,
                                    DateTime? AvailableFrom,
                                    DateTime? ExpiresAt,
                                    ListingStatus? Status);

// ── Response ──────────────────────────────────────────────────────────────────

public sealed record ListingResponseDto(
    Guid Id,
    Guid TenantId,
    Guid UserId,
    string Title,
    string? Description,
    ListingType Type,
    string TypeLabel,
    string LocationFrom,
    string LocationTo,
    decimal Price,
    string Currency,
    ListingStatus Status,
    string StatusLabel,
    decimal? WeightKg,
    decimal? VolumeM3,
    string? CargoType,
    DateTime? AvailableFrom,
    DateTime? ExpiresAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

// ── Search / filter ───────────────────────────────────────────────────────────


public sealed record ListingSearchDto
{
    // ── Location filters ───────────────────────────────────────────────────────
    public string? LocationFrom { get; init; }
    public string? LocationTo { get; init; }

    // ── Type filter ────────────────────────────────────────────────────────────
    public ListingType? Type { get; init; }

    // ── Keyword — matches Title, Description, CargoType ───────────────────────
    public string? Keyword { get; init; }

    // ── Price range ────────────────────────────────────────────────────────────
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }

    // ── Status filter (defaults to Active for public search) ───────────────────
    public ListingStatus Status { get; init; } = ListingStatus.Active;

    // ── Date range ─────────────────────────────────────────────────────────────
    public DateTime? AvailableFrom { get; init; }
    public DateTime? AvailableTo { get; init; }

    // ── Sorting ────────────────────────────────────────────────────────────────
    public ListingSortBy SortBy { get; init; } = ListingSortBy.Newest;

    // ── Pagination ─────────────────────────────────────────────────────────────
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    // ── Validated, capped values used by the repository ───────────────────────
    public int SafePage => Page < 1 ? 1 : Page;
    public int SafePageSize => PageSize < 1 ? 20
                             : PageSize > 100 ? 100
                             : PageSize;
}

public enum ListingSortBy
{
    Newest,        // CreatedAt DESC  (default)
    Oldest,        // CreatedAt ASC
    PriceAsc,      // Price ASC
    PriceDesc,     // Price DESC
    ExpiresFirst   // ExpiresAt ASC — shows listings expiring soonest first
}