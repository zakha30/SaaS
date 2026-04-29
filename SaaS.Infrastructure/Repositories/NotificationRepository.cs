using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Data;
using SaaS.Modules.Notifications.Entities;
using SaaS.Modules.Notifications.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Repositories;

public sealed class NotificationRepository(AppDbContext db) : INotificationRepository
{
    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        Guid userId, CancellationToken ct = default) =>
        await db.Notifications.AsNoTracking()
                               .Where(n => n.RecipientUserId == userId)
                               .OrderByDescending(n => n.CreatedAt)
                               .ToListAsync(ct);

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default) =>
        await db.Notifications.CountAsync(n => n.RecipientUserId == userId && !n.IsRead, ct);

    public async Task AddAsync(Notification notification, CancellationToken ct = default) =>
        await db.Notifications.AddAsync(notification, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}
