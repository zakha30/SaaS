using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Loads.Entities;

namespace SaaS.Modules.Loads.Repositories;

public interface IQuoteSubmissionRepository
{
    Task AddAsync(QuoteSubmission submission, System.Threading.CancellationToken ct = default);
    Task SaveChangesAsync(System.Threading.CancellationToken ct = default);
}
