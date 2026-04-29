using SaaS.Modules.Tenants.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Tenants.Services;

public interface ITenantService
{
    Task<Result<TenantResponseDto>> CreateAsync(CreateTenantDto dto, CancellationToken ct = default);
    Task<Result<TenantResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<TenantResponseDto>> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Result<IReadOnlyList<TenantResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<TenantResponseDto>> UpdateAsync(Guid id, UpdateTenantDto dto, CancellationToken ct = default);
    Task<Result<bool>> SuspendAsync(Guid id, CancellationToken ct = default);
    Task<Result<bool>> ActivateAsync(Guid id, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
}