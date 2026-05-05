using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Directory.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Directory.Services;

public interface IDirectoryService
{
    Task<PagedResult<DirectoryResponseDto>> GetFilteredAsync(DirectoryFilterDto filter, CancellationToken ct = default);
    Task<DirectoryResponseDto?> GetByIdAsync(System.Guid id, CancellationToken ct = default);
    Task<DirectoryResponseDto> CreateAsync(CreateDirectoryEntryDto dto, CancellationToken ct = default);
}
