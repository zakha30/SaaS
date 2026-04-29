using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Loads.Entities;

namespace SaaS.Modules.Loads.Repositories;

public interface IQuoteRequestRepository
{
    Task AddAsync(QuoteRequest request, System.Threading.CancellationToken ct = default);
    Task<QuoteRequest?> GetByIdAsync(System.Guid id, System.Threading.CancellationToken ct = default);
    Task SaveChangesAsync(System.Threading.CancellationToken ct = default);
}
