using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Loads.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Loads.Services;

public interface IAvailableLoadService
{
    Task<PagedResult<LoadResponseDto>> GetFilteredAsync(LoadFilterDto filter, CancellationToken ct = default);
    Task<LoadResponseDto> CreateAsync(CreateLoadDto dto, CancellationToken ct = default);
}
