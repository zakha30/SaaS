using System;
using SaaS.Shared;

namespace SaaS.Modules.Loads.Entities;

public sealed class QuoteSubmission : TenantEntity
{
    public Guid QuoteRequestId { get; set; }
    public Guid SubmittedByUserId { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}
