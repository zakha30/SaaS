using AutoMapper;
using SaaS.Modules.Listings.DTOs;
using SaaS.Modules.Listings.Entities;
using SaaS.Modules.Listings.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Listings.Services;

public sealed class ListingService(
    IListingRepository repository,
    IMapper mapper) : IListingService
{
    // ── Create ─────────────────────────────────────────────────────────────────

    public async Task<Result<ListingResponseDto>> CreateAsync(
        CreateListingDto dto, Guid userId, CancellationToken ct = default)
    {
        if (dto.ExpiresAt.HasValue && dto.ExpiresAt.Value <= DateTime.UtcNow)
            return Result<ListingResponseDto>.Failure(
                "ExpiresAt must be a future date.");

        if (dto.AvailableFrom.HasValue && dto.ExpiresAt.HasValue
            && dto.AvailableFrom.Value >= dto.ExpiresAt.Value)
            return Result<ListingResponseDto>.Failure(
                "AvailableFrom must be before ExpiresAt.");

        var listing = mapper.Map<Listing>(dto);
        listing.UserId = userId;

        await repository.AddAsync(listing, ct);
        await repository.SaveChangesAsync(ct);

        return Result<ListingResponseDto>.Success(
            mapper.Map<ListingResponseDto>(listing));
    }

    // ── GetById ────────────────────────────────────────────────────────────────

    public async Task<Result<ListingResponseDto>> GetByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        var listing = await repository.GetByIdAsync(id, ct);

        return listing is null
            ? Result<ListingResponseDto>.Failure("Listing not found.")
            : Result<ListingResponseDto>.Success(
                mapper.Map<ListingResponseDto>(listing));
    }

    // ── GetPaged ───────────────────────────────────────────────────────────────

    public async Task<Result<PagedResult<ListingResponseDto>>> GetPagedAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20
                 : pageSize > 100 ? 100
                 : pageSize;

        var paged = await repository.GetPagedAsync(page, pageSize, ct);

        return Result<PagedResult<ListingResponseDto>>.Success(
            new PagedResult<ListingResponseDto>(
                paged.Items.Select(mapper.Map<ListingResponseDto>).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize));
    }

    // ── Search ─────────────────────────────────────────────────────────────────

    public async Task<Result<ListingSearchResultDto>> SearchAsync(
        ListingSearchDto search, CancellationToken ct = default)
    {
        // Validate price range
        if (search.MinPrice.HasValue && search.MaxPrice.HasValue
            && search.MinPrice.Value > search.MaxPrice.Value)
            return Result<ListingSearchResultDto>.Failure(
                "MinPrice cannot be greater than MaxPrice.");

        // Validate availability window
        if (search.AvailableFrom.HasValue && search.AvailableTo.HasValue
            && search.AvailableFrom.Value > search.AvailableTo.Value)
            return Result<ListingSearchResultDto>.Failure(
                "AvailableFrom cannot be after AvailableTo.");

        var paged = await repository.SearchAsync(search, ct);

        var totalPages = (int)Math.Ceiling(
            paged.TotalCount / (double)paged.PageSize);

        var result = new ListingSearchResultDto(
            Items: paged.Items
                                  .Select(mapper.Map<ListingResponseDto>)
                                  .ToList(),
            TotalCount: paged.TotalCount,
            Page: paged.Page,
            PageSize: paged.PageSize,
            TotalPages: totalPages,
            HasNextPage: paged.Page < totalPages,
            HasPreviousPage: paged.Page > 1,
            AppliedFilters: BuildAppliedFilters(search));

        return Result<ListingSearchResultDto>.Success(result);
    }

    // ── Update ─────────────────────────────────────────────────────────────────

    public async Task<Result<ListingResponseDto>> UpdateAsync(
        Guid id, UpdateListingDto dto, Guid requestingUserId,
        CancellationToken ct = default)
    {
        var listing = await repository.GetByIdAsync(id, ct);
        if (listing is null)
            return Result<ListingResponseDto>.Failure("Listing not found.");

        if (listing.UserId != requestingUserId)
            return Result<ListingResponseDto>.Failure(
                "You do not have permission to update this listing.");

        if (listing.Status == ListingStatus.Archived)
            return Result<ListingResponseDto>.Failure(
                "Archived listings cannot be updated.");

        if (dto.ExpiresAt.HasValue && dto.ExpiresAt.Value <= DateTime.UtcNow)
            return Result<ListingResponseDto>.Failure(
                "ExpiresAt must be a future date.");

        mapper.Map(dto, listing);
        await repository.SaveChangesAsync(ct);

        return Result<ListingResponseDto>.Success(
            mapper.Map<ListingResponseDto>(listing));
    }

    // ── ChangeStatus ───────────────────────────────────────────────────────────

    public async Task<Result<ListingResponseDto>> ChangeStatusAsync(
        Guid id, ListingStatus status, Guid requestingUserId,
        CancellationToken ct = default)
    {
        var listing = await repository.GetByIdAsync(id, ct);
        if (listing is null)
            return Result<ListingResponseDto>.Failure("Listing not found.");

        if (listing.UserId != requestingUserId)
            return Result<ListingResponseDto>.Failure(
                "You do not have permission to change this listing's status.");

        var allowed = listing.Status switch
        {
            ListingStatus.Draft => new[] { ListingStatus.Active },
            ListingStatus.Active => new[] { ListingStatus.Paused, ListingStatus.Archived },
            ListingStatus.Paused => new[] { ListingStatus.Active, ListingStatus.Archived },
            ListingStatus.Archived => Array.Empty<ListingStatus>(),
            _ => Array.Empty<ListingStatus>()
        };

        if (!allowed.Contains(status))
            return Result<ListingResponseDto>.Failure(
                $"Cannot transition from '{listing.Status}' to '{status}'. " +
                $"Allowed: {string.Join(", ", allowed.Select(s => s.ToString()))}");

        listing.Status = status;
        await repository.SaveChangesAsync(ct);

        return Result<ListingResponseDto>.Success(
            mapper.Map<ListingResponseDto>(listing));
    }

    // ── Delete ─────────────────────────────────────────────────────────────────

    public async Task<Result<bool>> DeleteAsync(
        Guid id, Guid requestingUserId, CancellationToken ct = default)
    {
        var listing = await repository.GetByIdAsync(id, ct);
        if (listing is null)
            return Result<bool>.Failure("Listing not found.");

        if (listing.UserId != requestingUserId)
            return Result<bool>.Failure(
                "You do not have permission to delete this listing.");

        listing.IsDeleted = true;
        listing.Status = ListingStatus.Archived;

        await repository.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    // ── Private helpers ────────────────────────────────────────────────────────

    private static AppliedFiltersDto BuildAppliedFilters(ListingSearchDto s) => new(
        Keyword: s.Keyword,
        LocationFrom: s.LocationFrom,
        LocationTo: s.LocationTo,
        Type: s.Type?.ToString(),
        MinPrice: s.MinPrice,
        MaxPrice: s.MaxPrice,
        SortBy: s.SortBy.ToString(),
        Status: s.Status.ToString());
}