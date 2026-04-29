using System;
using SaaS.Shared;

namespace SaaS.Modules.Forum.Entities;

public sealed class ForumPost : TenantEntity
{
    public Guid ThreadId { get; set; }
    public Guid? PostedByUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}
