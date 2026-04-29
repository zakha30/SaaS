using SaaS.Shared;

namespace SaaS.Modules.Tenants.Entities;

public sealed class Plan : BaseEntity  // Global — no TenantId
{
    public string Name { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;  // Starter, Pro, Enterprise
    public decimal MonthlyPrice { get; set; }
    public int MaxUsers { get; set; }
    public int MaxListings { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Tenant> Tenants { get; set; } = [];
}
