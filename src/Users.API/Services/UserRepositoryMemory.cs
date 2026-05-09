using Users.API.Models;

namespace Users.API.Services;

public class UserRepositoryMemory : IUserRepository
{
    // Lista en memoria que simula la base de datos
    private static List<User> _users = new();

    // Busca un usuario por email
    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email == email);
    }

    // Agrega un usuario a la lista
    public void Add(User user)
    {
        _users.Add(user);
    }

    // Actualiza un usuario
    public void Update(User user)
    {
        // En memoria no hace falta hacer nada
        // porque los objetos se modifican por referencia
    }
}