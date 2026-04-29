using ENIT.BL.Entities;

namespace ENIT.DAL.Contracts;

public interface INotificationManager
{
    Task<IEnumerable<Notification>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default);
}
