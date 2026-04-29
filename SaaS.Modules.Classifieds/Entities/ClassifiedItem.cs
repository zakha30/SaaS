using System;
using SaaS.Shared;

namespace SaaS.Modules.Classifieds.Entities;

public sealed class ClassifiedItem : TenantEntity
{
    public Guid? PostedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? City { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = "Active";
    public string MembershipTier { get; set; } = "Free";
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(90);
}
