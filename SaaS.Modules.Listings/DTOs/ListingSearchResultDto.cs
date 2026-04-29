namespace SaaS.Modules.Listings.DTOs;

public sealed record ListingSearchResultDto(
    IReadOnlyList<ListingResponseDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage,
    AppliedFiltersDto AppliedFilters);

public sealed record AppliedFiltersDto(
    string? Keyword,
    string? LocationFrom,
    string? LocationTo,
    string? Type,
    decimal? MinPrice,
    decimal? MaxPrice,
    string SortBy,
    string Status);