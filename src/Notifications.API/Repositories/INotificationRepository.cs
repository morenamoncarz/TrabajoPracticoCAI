using Notifications.API.Models;

//qué cosas sabe hacer el repository, no como.

public interface INotificationRepository
{
    Task AddAsync(Notification notification);

    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
}