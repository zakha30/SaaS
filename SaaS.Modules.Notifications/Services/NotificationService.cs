using SaaS.Modules.Notifications.DTOs;
using SaaS.Modules.Notifications.Entities;
using SaaS.Modules.Notifications.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Notifications.Services;

public sealed class NotificationService(
    INotificationRepository repository,
    IEmailSender emailSender) : INotificationService
{
    public async Task<Result<NotificationResponseDto>> SendAsync(
        SendNotificationDto dto, CancellationToken ct = default)
    {
        var notification = new Notification
        {
            RecipientUserId = dto.RecipientUserId,
            Subject = dto.Subject,
            Body = dto.Body,
            Type = dto.Type,
            RelatedQuoteId = dto.RelatedQuoteId,
            RelatedListingId = dto.RelatedListingId
        };

        await repository.AddAsync(notification, ct);

        if (dto.Type == NotificationType.Email)
        {
            await emailSender.SendAsync(dto.RecipientEmail, dto.Subject, dto.Body, ct);
            notification.IsSent = true;
            notification.SentAt = DateTime.UtcNow;
        }

        await repository.SaveChangesAsync(ct);
        return Result<NotificationResponseDto>.Success(MapToDto(notification));
    }

    public async Task<Result<IReadOnlyList<NotificationResponseDto>>> GetForUserAsync(
        Guid userId, CancellationToken ct = default)
    {
        var notifications = await repository.GetByUserIdAsync(userId, ct);
        return Result<IReadOnlyList<NotificationResponseDto>>.Success(
            notifications.Select(MapToDto).ToList());
    }

    public async Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var count = await repository.GetUnreadCountAsync(userId, ct);
        return Result<int>.Success(count);
    }

    public async Task<Result<bool>> MarkAsReadAsync(Guid id, CancellationToken ct = default)
    {
        var notification = await repository.GetByIdAsync(id, ct);
        if (notification is null) return Result<bool>.Failure("Notification not found.");

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    private static NotificationResponseDto MapToDto(Notification n) =>
        new(n.Id, n.TenantId, n.RecipientUserId, n.Subject, n.Body,
            n.Type, n.IsRead, n.IsSent, n.RelatedQuoteId, n.RelatedListingId,
            n.SentAt, n.CreatedAt);
}
