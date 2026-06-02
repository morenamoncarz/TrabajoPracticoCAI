using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs;


/// Datos que el cliente envía para registrar un usuario.

public class RegisterUserRequest
{
    [Required]
    public string Nombre { get; set; } = "";

    [Required]
    public string Apellido { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}