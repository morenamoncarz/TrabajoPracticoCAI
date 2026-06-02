using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;
using Notifications.API.Repositories;

namespace Notifications.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly UsersApiClient _usersApiClient;

    public NotificationService(
        INotificationRepository repo,
        UsersApiClient usersApiClient)
    {
        _repo = repo;
        _usersApiClient = usersApiClient;
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

        // ntf-001: el usuario tiene que existir en users api
        var existeUsuario = await _usersApiClient.UserExists(request.UsuarioId);

        if (!existeUsuario)
        {
            throw new NotFoundException(
                "NTF-001",
                "El usuario destinatario no fue encontrado."
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