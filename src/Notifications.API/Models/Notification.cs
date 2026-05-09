using System.ComponentModel.DataAnnotations;

namespace Notifications.API.Models;

/// <summary>Notificacion enviada a un usuario.</summary>
public class Notification
{
	public Guid Id { get; set; }

	/// <summary>Usuario destinatario de la notificacion.</summary>
	[Required]
	public Guid UsuarioId { get; set; }

	/// <summary>Mensaje de la notificacion.</summary>
	[Required]
	[StringLength(500)]
	public string Mensaje { get; set; } = "";

	/// <summary>Tipo de notificacion: Email, Push o SMS.</summary>
	[Required]
	public string Tipo { get; set; } = "";

	/// <summary>Estado de la notificacion.</summary>
	public string Estado { get; set; } = "";

	/// <summary>Fecha en la que se registro el envio.</summary>
	public DateTime FechaEnvio { get; set; }
}