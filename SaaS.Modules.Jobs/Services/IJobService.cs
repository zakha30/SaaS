using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Jobs.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Jobs.Services;

public interface IJobService
{
    Task<PagedResult<JobResponseDto>> GetFilteredAsync(JobFilterDto filter, CancellationToken ct = default);
    Task<JobResponseDto> CreateAsync(CreateJobDto dto, CancellationToken ct = default);
}
