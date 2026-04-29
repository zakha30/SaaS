using System.ComponentModel.DataAnnotations;

namespace SaaS.Modules.Tenants.DTOs;

public sealed record CreateTenantDto(
    [Required, MinLength(2), MaxLength(200)] string Name,
    [Required, MinLength(2), MaxLength(100)] string Slug,
    [Required, EmailAddress] string ContactEmail,
    [Required] Guid PlanId);

public sealed record UpdateTenantDto(
    [MaxLength(200)] string? Name,
    [EmailAddress] string? ContactEmail,
                     string? Status,
                     Guid? PlanId);

public sealed record TenantResponseDto(
    Guid Id,
    string Name,
    string Slug,
    string ContactEmail,
    string Status,
    bool IsActive,
    Guid PlanId,
    DateTime? PlanExpiresAt,
    DateTime CreatedAt);

public sealed record TenantSettingDto(
    string Key,
    string Value);