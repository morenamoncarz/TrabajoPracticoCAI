using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs;


/// Datos que el cliente envía para iniciar sesión.

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}