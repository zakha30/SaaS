using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Classifieds.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Classifieds.Repositories;

public interface IClassifiedRepository
{
    Task<PagedResult<ClassifiedItem>> GetFilteredAsync(DTOs.ClassifiedFilterDto filter, CancellationToken ct = default);
    Task AddAsync(ClassifiedItem item, CancellationToken ct = default);
    Task<ClassifiedItem?> GetByIdAsync(System.Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
