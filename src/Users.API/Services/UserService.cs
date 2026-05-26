using System.Security.Cryptography;
using System.Text;
using Users.API.DTOs;
using Users.API.Exceptions;
using Users.API.Models;

namespace Users.API.Services;

public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public UserResponse Register(RegisterUserRequest request)
    {
        // Validamos si ya existe un usuario con ese email
        var existingUser = _repository.GetByEmail(request.Email);

        if (existingUser != null)
        {
            throw new BusinessRuleException(
                "USR-001",
                $"El email '{request.Email}' ya está registrado."
            );
        }

        // Creamos el usuario nuevo
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Email = request.Email,

            // Guardamos el hash y no la contraseña real
            PasswordHash = HashPassword(request.Password),

            FechaRegistro = DateTime.UtcNow,

            // En el modelo de C# Activo es bool
            Activo = true,

            IntentosFallidos = 0
        };

        _repository.Add(user);

        return MapToResponse(user);
    }

    public UserResponse Login(LoginRequest request)
    {
        // Buscamos el usuario por email
        var user = _repository.GetByEmail(request.Email);

        if (user == null)
        {
            throw new BusinessRuleException(
                "USR-003",
                "Credenciales incorrectas."
            );
        }

        // Si el usuario está bloqueado, no permitimos login
        if (!user.Activo)
        {
            throw new BusinessRuleException(
                "USR-004",
                "Usuario bloqueado por demasiados intentos fallidos."
            );
        }

        var passwordHash = HashPassword(request.Password);

        // Si la contraseña no coincide
        if (user.PasswordHash != passwordHash)
        {
            user.IntentosFallidos++;

            // Al tercer intento fallido bloqueamos el usuario
            if (user.IntentosFallidos >= 3)
            {
                user.Activo = false;

                _repository.Update(user);

                throw new BusinessRuleException(
                    "USR-004",
                    "Usuario bloqueado por demasiados intentos fallidos."
                );
            }

            _repository.Update(user);

            throw new BusinessRuleException(
                "USR-003",
                "Credenciales incorrectas."
            );
        }

        // Si el login es correcto reiniciamos intentos fallidos
        user.IntentosFallidos = 0;

        _repository.Update(user);

        return MapToResponse(user);
    }

    private UserResponse MapToResponse(User user)
    {
        // Convertimos User a UserResponse para no devolver PasswordHash
        return new UserResponse
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Email = user.Email,
            FechaRegistro = user.FechaRegistro,
            Activo = user.Activo
        };
    }

    private string HashPassword(string password)
    {
        // Nunca guardamos la contraseña real
        using var sha256 = SHA256.Create();

        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}