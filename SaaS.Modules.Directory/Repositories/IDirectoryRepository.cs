using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Directory.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Directory.Repositories;

public interface IDirectoryRepository
{
    Task<PagedResult<BusinessDirectoryEntry>> GetFilteredAsync(DTOs.DirectoryFilterDto filter, CancellationToken ct = default);
    Task AddAsync(BusinessDirectoryEntry entry, CancellationToken ct = default);
    Task<BusinessDirectoryEntry?> GetByIdAsync(System.Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
