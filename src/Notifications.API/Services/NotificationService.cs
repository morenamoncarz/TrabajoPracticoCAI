using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;

namespace Notifications.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;

    public NotificationService(INotificationRepository repo)
    {
        _repo = repo;
    }

    public async Task<Notification> SendAsync(SendNotificationRequest request)
    {
        var tiposValidos = new[] { "Email", "SMS", "Push" };

        if (!tiposValidos.Contains(request.Tipo))
        {
            throw new ValidationException(
                "NTF-002",
                "El tipo de notificacion es invalido."
            );
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Mensaje = request.Mensaje,
            Tipo = request.Tipo,
            Estado = "Enviada",
            FechaEnvio = DateTime.UtcNow
        };

        await _repo.AddAsync(notification);

        return notification;
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
    {
        var notifications = await _repo.GetByUserIdAsync(userId);

        if (!notifications.Any())
        {
            throw new NotFoundException(
                "NTF-003",
                "No se encontraron notificaciones para el usuario."
            );
        }

        return notifications;
    }
}