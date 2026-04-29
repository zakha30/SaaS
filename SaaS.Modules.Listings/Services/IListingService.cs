using SaaS.Modules.Listings.DTOs;
using SaaS.Modules.Listings.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Listings.Services;

public interface IListingService
{
    Task<Result<ListingResponseDto>> CreateAsync(CreateListingDto dto, Guid userId, CancellationToken ct = default);
    Task<Result<ListingResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PagedResult<ListingResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Result<ListingSearchResultDto>> SearchAsync(ListingSearchDto search, CancellationToken ct = default);
    Task<Result<ListingResponseDto>> UpdateAsync(Guid id, UpdateListingDto dto, Guid requestingUserId, CancellationToken ct = default);
    Task<Result<ListingResponseDto>> ChangeStatusAsync(Guid id, ListingStatus status, Guid requestingUserId, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(Guid id, Guid requestingUserId, CancellationToken ct = default);
}