using SaaS.Shared;

namespace SaaS.Modules.Quotes.Entities;

public sealed class Quote : TenantEntity
{
    public Guid ListingId { get; set; }
    public Guid TransporterId { get; set; }  // UserId of the bidder
    public decimal Price { get; set; }
    public string Message { get; set; } = string.Empty;
    public QuoteStatus Status { get; set; } = QuoteStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? WithdrawnAt { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? ValidUntil { get; set; }
}

public enum QuoteStatus
{
    Pending,    // submitted, awaiting decision
    Accepted,   // listing owner accepted
    Rejected,   // listing owner rejected
    Withdrawn,  // transporter withdrew the quote
    Expired     // ValidUntil passed without decision
}