namespace SaaS.Modules.Tenants.DTOs;

public sealed record CreatePlanDto(
    string Name,
    string Tier,
    decimal MonthlyPrice,
    int MaxUsers,
    int MaxListings);

public sealed record PlanResponseDto(
    Guid Id,
    string Name,
    string Tier,
    decimal MonthlyPrice,
    int MaxUsers,
    int MaxListings,
    bool IsActive,
    DateTime CreatedAt);
