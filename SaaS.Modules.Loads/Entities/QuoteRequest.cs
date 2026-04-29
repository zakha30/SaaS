using System;
using SaaS.Shared;

namespace SaaS.Modules.Loads.Entities;

public sealed class QuoteRequest : TenantEntity
{
    public Guid? RequestedByUserId { get; set; }
    public Guid LoadId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public decimal? Budget { get; set; }
    public string Currency { get; set; } = "ZAR";
    public DateTime? NeededBy { get; set; }
    public string Status { get; set; } = "Open";
}
