using Notifications.API.Models;

namespace Notifications.API.Services;

public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly List<Notification> _datos = new();

    public Task AddAsync(Notification notification)
    {
        _datos.Add(notification);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
    {
        var notificaciones = _datos
            .Where(n => n.UsuarioId == userId)
            .OrderByDescending(n => n.FechaEnvio);

        return Task.FromResult<IEnumerable<Notification>>(notificaciones);
    }
}