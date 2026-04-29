using SaaS.Modules.Notifications.Entities;

namespace SaaS.Modules.Notifications.DTOs;

public sealed record SendNotificationDto(
    Guid RecipientUserId,
    string RecipientEmail,
    string Subject,
    string Body,
    NotificationType Type = NotificationType.InApp,
    Guid? RelatedQuoteId = null,
    Guid? RelatedListingId = null);

public sealed record NotificationResponseDto(
    Guid Id,
    Guid TenantId,
    Guid RecipientUserId,
    string Subject,
    string Body,
    NotificationType Type,
    bool IsRead,
    bool IsSent,
    Guid? RelatedQuoteId,
    Guid? RelatedListingId,
    DateTime? SentAt,
    DateTime CreatedAt);
