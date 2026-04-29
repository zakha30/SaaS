using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Jobs.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Jobs.Repositories;

public interface IJobRepository
{
    Task<PagedResult<JobListing>> GetFilteredAsync(DTOs.JobFilterDto filter, CancellationToken ct = default);
    Task AddAsync(JobListing job, CancellationToken ct = default);
    Task<JobListing?> GetByIdAsync(System.Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
