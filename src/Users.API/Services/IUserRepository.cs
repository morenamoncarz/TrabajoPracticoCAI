using Users.API.Models;

namespace Users.API.Services;

public interface IUserRepository
{
    // Busca un usuario por email
    User? GetByEmail(string email);

    // Busca un usuario por id
    User? GetById(Guid id);

    // Guarda un nuevo usuario
    void Add(User user);

    // Actualiza un usuario existente
    void Update(User user);
}