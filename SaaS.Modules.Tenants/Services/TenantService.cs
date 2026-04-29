using SaaS.Modules.Tenants.DTOs;
using SaaS.Modules.Tenants.Entities;
using SaaS.Modules.Tenants.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Tenants.Services;

public sealed class TenantService(ITenantRepository repository) : ITenantService
{
    public async Task<Result<TenantResponseDto>> CreateAsync(
        CreateTenantDto dto, CancellationToken ct = default)
    {
        // Enforce uniqueness of slug and email across all tenants
        if (await repository.SlugExistsAsync(dto.Slug, ct))
            return Result<TenantResponseDto>.Failure(
                $"Slug '{dto.Slug}' is already taken. Choose a different slug.");

        if (await repository.EmailExistsAsync(dto.ContactEmail, ct))
            return Result<TenantResponseDto>.Failure(
                $"A tenant with email '{dto.ContactEmail}' already exists.");

        var tenant = new Tenant
        {
            Name = dto.Name.Trim(),
            Slug = dto.Slug.ToLowerInvariant().Trim(),
            ContactEmail = dto.ContactEmail.ToLowerInvariant().Trim(),
            PlanId = dto.PlanId,
            Status = TenantStatus.Active
        };

        await repository.AddAsync(tenant, ct);
        await repository.SaveChangesAsync(ct);

        return Result<TenantResponseDto>.Success(MapToDto(tenant));
    }

    public async Task<Result<TenantResponseDto>> GetByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        var tenant = await repository.GetByIdAsync(id, ct);

        return tenant is null
            ? Result<TenantResponseDto>.Failure("Tenant not found.")
            : Result<TenantResponseDto>.Success(MapToDto(tenant));
    }

    public async Task<Result<TenantResponseDto>> GetBySlugAsync(
        string slug, CancellationToken ct = default)
    {
        var tenant = await repository.GetBySlugAsync(slug, ct);

        return tenant is null
            ? Result<TenantResponseDto>.Failure($"Tenant with slug '{slug}' not found.")
            : Result<TenantResponseDto>.Success(MapToDto(tenant));
    }

    public async Task<Result<IReadOnlyList<TenantResponseDto>>> GetAllAsync(
        CancellationToken ct = default)
    {
        var tenants = await repository.GetAllAsync(ct);
        return Result<IReadOnlyList<TenantResponseDto>>.Success(
            tenants.Select(MapToDto).ToList());
    }

    public async Task<Result<TenantResponseDto>> UpdateAsync(
        Guid id, UpdateTenantDto dto, CancellationToken ct = default)
    {
        var tenant = await repository.GetByIdAsync(id, ct);
        if (tenant is null)
            return Result<TenantResponseDto>.Failure("Tenant not found.");

        // Validate status transition if provided
        if (dto.Status is not null &&
            dto.Status is not TenantStatus.Active
                       and not TenantStatus.Suspended
                       and not TenantStatus.Cancelled)
        {
            return Result<TenantResponseDto>.Failure(
                $"Invalid status '{dto.Status}'. Valid values: Active, Suspended, Cancelled.");
        }

        if (dto.Name is not null) tenant.Name = dto.Name.Trim();
        if (dto.ContactEmail is not null) tenant.ContactEmail = dto.ContactEmail.ToLowerInvariant().Trim();
        if (dto.Status is not null) tenant.Status = dto.Status;
        if (dto.PlanId is not null) tenant.PlanId = dto.PlanId.Value;

        await repository.SaveChangesAsync(ct);
        return Result<TenantResponseDto>.Success(MapToDto(tenant));
    }

    public async Task<Result<bool>> SuspendAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await repository.GetByIdAsync(id, ct);
        if (tenant is null)
            return Result<bool>.Failure("Tenant not found.");

        if (tenant.Status == TenantStatus.Suspended)
            return Result<bool>.Failure("Tenant is already suspended.");

        tenant.Status = TenantStatus.Suspended;
        await repository.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ActivateAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await repository.GetByIdAsync(id, ct);
        if (tenant is null)
            return Result<bool>.Failure("Tenant not found.");

        if (tenant.Status == TenantStatus.Active)
            return Result<bool>.Failure("Tenant is already active.");

        tenant.Status = TenantStatus.Active;
        await repository.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await repository.GetByIdAsync(id, ct);
        if (tenant is null)
            return Result<bool>.Failure("Tenant not found.");

        tenant.IsDeleted = true;
        tenant.Status = TenantStatus.Cancelled;
        await repository.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    private static TenantResponseDto MapToDto(Tenant t) => new(
        t.Id,
        t.Name,
        t.Slug,
        t.ContactEmail,
        t.Status,
        t.IsActive,
        t.PlanId,
        t.PlanExpiresAt,
        t.CreatedAt);
}