using SaaS.Modules.Tenants.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Tenants.Services;

public interface IPlanService
{
    Task<Result<PlanResponseDto>> CreateAsync(CreatePlanDto dto, CancellationToken ct = default);
    Task<Result<IReadOnlyList<PlanResponseDto>>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Result<PlanResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
}
