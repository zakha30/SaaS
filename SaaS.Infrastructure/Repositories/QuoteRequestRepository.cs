using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Loads.Entities;
using SaaS.Modules.Loads.Repositories;

namespace SaaS.Infrastructure.Repositories;

public sealed class QuoteRequestRepository : IQuoteRequestRepository
{
    private readonly AppDbContext db;
    public QuoteRequestRepository(AppDbContext db) => this.db = db;

    public async Task AddAsync(QuoteRequest request, CancellationToken ct = default) => await db.Set<QuoteRequest>().AddAsync(request, ct);

    public async Task<QuoteRequest?> GetByIdAsync(Guid id, CancellationToken ct = default) => await db.Set<QuoteRequest>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) => await db.SaveChangesAsync(ct);
}
