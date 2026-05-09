using System.ComponentModel.DataAnnotations;

namespace Users.API.Models;


/// Representa un usuario del sistema.

public class User
{
    public Guid Id { get; set; }

    [Required]
    public string Nombre { get; set; } = "";

    [Required]
    public string Apellido { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    // Guardamos la contraseña en formato "hash"
    // (no guardamos la contraseña real)
    public string PasswordHash { get; set; } = "";

    public DateTime FechaRegistro { get; set; }

    // Si está en false, el usuario está bloqueado
    public bool Activo { get; set; } = true;

    // Contador de intentos fallidos de login
    public int IntentosFallidos { get; set; }
}