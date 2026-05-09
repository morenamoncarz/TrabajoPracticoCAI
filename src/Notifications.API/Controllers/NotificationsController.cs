using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Models;
using Notifications.API.Services;

namespace Notifications.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    /// <summary>Registra y simula el envio de una notificacion.</summary>
    [HttpPost("send")]
    [ProducesResponseType(typeof(Notification), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
    {
        var notification = await _service.SendAsync(request);

        return CreatedAtAction(
            nameof(GetByUserId),
            new { userId = notification.UsuarioId },
            notification
        );
    }

    /// <summary>Lista las notificaciones de un usuario.</summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var notifications = await _service.GetByUserIdAsync(userId);

        return Ok(notifications);
    }
}