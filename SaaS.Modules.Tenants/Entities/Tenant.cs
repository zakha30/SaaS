using SaaS.Shared;

namespace SaaS.Modules.Tenants.Entities;

public sealed class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string Status { get; set; } = TenantStatus.Active;
    public Guid PlanId { get; set; }
    public bool IsActive => Status == TenantStatus.Active;
    public DateTime? PlanExpiresAt { get; set; }

    public ICollection<TenantSettings> Settings { get; set; } = [];
}

public static class TenantStatus
{
    public const string Active = "Active";
    public const string Suspended = "Suspended";
    public const string Cancelled = "Cancelled";
}

public sealed class TenantSettings : TenantEntity
{
    public Guid TenantId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}