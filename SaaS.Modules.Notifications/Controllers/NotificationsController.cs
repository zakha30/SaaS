using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Notifications.Services;

namespace SaaS.Modules.Notifications.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController(INotificationService service) : ControllerBase
{
    private Guid UserId
    {
        get
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User identity claim is missing.");
            return Guid.Parse(value);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetForCurrentUser(CancellationToken ct)
    {
        var result = await service.GetForUserAsync(UserId, ct);
        return Ok(result.Value);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount(CancellationToken ct)
    {
        var result = await service.GetUnreadCountAsync(UserId, ct);
        return Ok(new { count = result.Value });
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        var result = await service.MarkAsReadAsync(id, ct);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}