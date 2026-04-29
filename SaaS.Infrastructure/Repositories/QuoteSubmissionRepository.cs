using System.Threading;
using System.Threading.Tasks;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Loads.Entities;
using SaaS.Modules.Loads.Repositories;

namespace SaaS.Infrastructure.Repositories;

public sealed class QuoteSubmissionRepository : IQuoteSubmissionRepository
{
    private readonly AppDbContext db;
    public QuoteSubmissionRepository(AppDbContext db) => this.db = db;

    public async Task AddAsync(QuoteSubmission submission, CancellationToken ct = default) => await db.Set<QuoteSubmission>().AddAsync(submission, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) => await db.SaveChangesAsync(ct);
}
