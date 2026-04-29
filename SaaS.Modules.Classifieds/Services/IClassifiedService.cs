using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Classifieds.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Classifieds.Services;

public interface IClassifiedService
{
    Task<PagedResult<ClassifiedResponseDto>> GetFilteredAsync(ClassifiedFilterDto filter, CancellationToken ct = default);
    Task<ClassifiedResponseDto> CreateAsync(CreateClassifiedDto dto, CancellationToken ct = default);
}
