using System;
using System.Collections.Generic;
using SaaS.Shared;

namespace SaaS.Modules.Forum.Entities;

public sealed class ForumThread : TenantEntity
{
    public Guid? PostedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string MembershipTier { get; set; } = "Free";
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(365);
}
