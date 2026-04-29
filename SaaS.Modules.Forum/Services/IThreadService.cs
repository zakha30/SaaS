using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Forum.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Forum.Services;

public interface IThreadService
{
    Task<PagedResult<ThreadResponseDto>> GetFilteredAsync(ThreadFilterDto filter, CancellationToken ct = default);
    Task<ThreadResponseDto> CreateAsync(CreateThreadDto dto, CancellationToken ct = default);
}
