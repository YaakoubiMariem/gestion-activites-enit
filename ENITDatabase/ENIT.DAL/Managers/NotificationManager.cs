using ENIT.BL.Entities;
using ENIT.DAL.Context;
using ENIT.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ENIT.DAL.Managers;

public class NotificationManager : INotificationManager
{
    private readonly ENITDbContext _context;

    public NotificationManager(ENITDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
        return notification;
    }

    public async Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.IsRead = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
