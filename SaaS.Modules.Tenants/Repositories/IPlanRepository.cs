using SaaS.Modules.Tenants.Entities;

namespace SaaS.Modules.Tenants.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Plan>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Plan plan, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
