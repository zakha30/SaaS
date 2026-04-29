using SaaS.Modules.Listings.DTOs;
using SaaS.Modules.Listings.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Listings.Repositories;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Listing>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<PagedResult<Listing>> SearchAsync(ListingSearchDto search, CancellationToken ct = default);
    Task<int> CountByStatusAsync(ListingStatus status, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Listing listing, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}