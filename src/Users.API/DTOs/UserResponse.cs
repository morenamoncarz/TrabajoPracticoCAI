namespace Users.API.DTOs;


/// Datos del usuario que la API devuelve como respuesta.
/// No incluye PasswordHash por seguridad.

public class UserResponse
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = "";

    public string Apellido { get; set; } = "";

    public string Email { get; set; } = "";

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }
}