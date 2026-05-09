using Notifications.API.DTOs;
using Notifications.API.Models;

namespace Notifications.API.Services;

public interface INotificationService
{
    Task<Notification> SendAsync(SendNotificationRequest request);

    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
}