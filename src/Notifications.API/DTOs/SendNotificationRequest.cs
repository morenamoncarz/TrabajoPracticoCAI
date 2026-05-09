using System.ComponentModel.DataAnnotations;

namespace Notifications.API.DTOs;

public class SendNotificationRequest
{
    [Required]
    public Guid UsuarioId { get; set; }

    [Required]
    [StringLength(500)]
    public string Mensaje { get; set; } = "";

    [Required]
    public string Tipo { get; set; } = "";
}