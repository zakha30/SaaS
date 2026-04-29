using SaaS.Modules.Quotes.DTOs;
using SaaS.Modules.Quotes.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Quotes.Repositories;

public interface IQuoteRepository
{
    Task<Quote?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Quote>> GetByFilterAsync(QuoteFilterDto filter, CancellationToken ct = default);
    Task<bool> HasPendingQuoteAsync(Guid listingId, Guid transporterId, CancellationToken ct = default);
    Task<bool> ListingHasAcceptedQuoteAsync(Guid listingId, CancellationToken ct = default);
    Task AddAsync(Quote quote, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}