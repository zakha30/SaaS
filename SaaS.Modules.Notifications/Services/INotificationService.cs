using SaaS.Modules.Notifications.DTOs;
using SaaS.Shared;

namespace SaaS.Modules.Notifications.Services;

public interface INotificationService
{
    Task<Result<NotificationResponseDto>> SendAsync(SendNotificationDto dto, CancellationToken ct = default);
    Task<Result<IReadOnlyList<NotificationResponseDto>>> GetForUserAsync(Guid userId, CancellationToken ct = default);
    Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task<Result<bool>> MarkAsReadAsync(Guid id, CancellationToken ct = default);
}

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}
