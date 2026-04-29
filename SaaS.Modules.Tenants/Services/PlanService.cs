using SaaS.Modules.Tenants.DTOs;
using SaaS.Modules.Tenants.Entities;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Tenants.Services;

public sealed class PlanService(IPlanRepository repository) : IPlanService
{
    public async Task<Result<PlanResponseDto>> CreateAsync(CreatePlanDto dto, CancellationToken ct = default)
    {
        var plan = new Plan
        {
            Name = dto.Name,
            Tier = dto.Tier,
            MonthlyPrice = dto.MonthlyPrice,
            MaxUsers = dto.MaxUsers,
            MaxListings = dto.MaxListings
        };

        await repository.AddAsync(plan, ct);
        await repository.SaveChangesAsync(ct);
        return Result<PlanResponseDto>.Success(MapToDto(plan));
    }

    public async Task<Result<IReadOnlyList<PlanResponseDto>>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var plans = await repository.GetAllActiveAsync(ct);
        return Result<IReadOnlyList<PlanResponseDto>>.Success(plans.Select(MapToDto).ToList());
    }

    public async Task<Result<PlanResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var plan = await repository.GetByIdAsync(id, ct);
        return plan is null
            ? Result<PlanResponseDto>.Failure("Plan not found.")
            : Result<PlanResponseDto>.Success(MapToDto(plan));
    }

    private static PlanResponseDto MapToDto(Plan p) =>
        new(p.Id, p.Name, p.Tier, p.MonthlyPrice, p.MaxUsers, p.MaxListings, p.IsActive, p.CreatedAt);
}
