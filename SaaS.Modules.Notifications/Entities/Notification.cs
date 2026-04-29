using SaaS.Shared;

namespace SaaS.Modules.Notifications.Entities;

public sealed class Notification : TenantEntity
{
    public Guid RecipientUserId { get; set; }
    public Guid? RelatedQuoteId { get; set; }
    public Guid? RelatedListingId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.InApp;
    public bool IsRead { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

public enum NotificationType { InApp, Email, SMS }
