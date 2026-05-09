using Notifications.API.Models;

namespace Notifications.API.Services;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
}