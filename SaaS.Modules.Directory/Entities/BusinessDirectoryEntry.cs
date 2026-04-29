using System;
using SaaS.Shared;

namespace SaaS.Modules.Directory.Entities;

public sealed class BusinessDirectoryEntry : TenantEntity
{
    public Guid? PostedByUserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? City { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string MembershipTier { get; set; } = "Free";
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(365);
}
