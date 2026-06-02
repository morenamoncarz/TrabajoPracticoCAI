using Dapper;
using Microsoft.Data.Sqlite;
using Users.API.Models;

namespace Users.API.Services;

public class UserRepositoryDb : IUserRepository
{
    private readonly IConfiguration _config;

    public UserRepositoryDb(IConfiguration config)
    {
        _config = config;
    }

    private SqliteConnection CreateConnection()
    {
        var connectionString =
            _config.GetConnectionString("DefaultConnection")
            ?? "Data Source=users.db";

        return new SqliteConnection(connectionString);
    }

    public User? GetByEmail(string email)
    {
        using var connection = CreateConnection();

        // Usamos una clase auxiliar con nombres distintos para evitar que Dapper intente mapear directo a User
        var row = connection.QueryFirstOrDefault<UserDbRow>("""
            SELECT
                id AS IdText,
                nombre AS NombreText,
                apellido AS ApellidoText,
                email AS EmailText,
                password_hash AS PasswordHashText,
                fecha_registro AS FechaRegistroText,
                activo AS ActivoNumber,
                intentos_fallidos AS IntentosFallidosNumber
            FROM users
            WHERE email = @Email
        """, new { Email = email });

        return MapToUser(row);
    }

    public User? GetById(Guid id)
    {
        using var connection = CreateConnection();

        // Buscamos el usuario por id para que otros microservicios puedan validarlo
        var row = connection.QueryFirstOrDefault<UserDbRow>("""
            SELECT
                id AS IdText,
                nombre AS NombreText,
                apellido AS ApellidoText,
                email AS EmailText,
                password_hash AS PasswordHashText,
                fecha_registro AS FechaRegistroText,
                activo AS ActivoNumber,
                intentos_fallidos AS IntentosFallidosNumber
            FROM users
            WHERE id = @Id
        """, new
        {
            Id = id.ToString()
        });

        return MapToUser(row);
    }

    public void Add(User user)
    {
        using var connection = CreateConnection();

        // Guardamos Guid y DateTime como texto, y bool como 1/0
        connection.Execute("""
            INSERT INTO users (
                id,
                nombre,
                apellido,
                email,
                password_hash,
                fecha_registro,
                activo,
                intentos_fallidos
            )
            VALUES (
                @Id,
                @Nombre,
                @Apellido,
                @Email,
                @PasswordHash,
                @FechaRegistro,
                @Activo,
                @IntentosFallidos
            )
        """, new
        {
            Id = user.Id.ToString(),
            user.Nombre,
            user.Apellido,
            user.Email,
            user.PasswordHash,
            FechaRegistro = user.FechaRegistro.ToString("O"),
            Activo = user.Activo ? 1 : 0,
            user.IntentosFallidos
        });
    }

    public void Update(User user)
    {
        using var connection = CreateConnection();

        // Actualizamos el usuario en SQLite
        connection.Execute("""
            UPDATE users
            SET
                nombre = @Nombre,
                apellido = @Apellido,
                email = @Email,
                password_hash = @PasswordHash,
                fecha_registro = @FechaRegistro,
                activo = @Activo,
                intentos_fallidos = @IntentosFallidos
            WHERE id = @Id
        """, new
        {
            Id = user.Id.ToString(),
            user.Nombre,
            user.Apellido,
            user.Email,
            user.PasswordHash,
            FechaRegistro = user.FechaRegistro.ToString("O"),
            Activo = user.Activo ? 1 : 0,
            user.IntentosFallidos
        });
    }

    private User? MapToUser(UserDbRow? row)
    {
        // Si no hay datos, devolvemos null
        if (row == null)
        {
            return null;
        }

        // Convertimos desde los datos de SQLite al modelo User
        return new User
        {
            Id = Guid.Parse(row.IdText),
            Nombre = row.NombreText,
            Apellido = row.ApellidoText,
            Email = row.EmailText,
            PasswordHash = row.PasswordHashText,
            FechaRegistro = DateTime.Parse(row.FechaRegistroText),
            Activo = row.ActivoNumber == 1,
            IntentosFallidos = row.IntentosFallidosNumber
        };
    }

    private class UserDbRow
    {
        public string IdText { get; set; } = "";
        public string NombreText { get; set; } = "";
        public string ApellidoText { get; set; } = "";
        public string EmailText { get; set; } = "";
        public string PasswordHashText { get; set; } = "";
        public string FechaRegistroText { get; set; } = "";
        public int ActivoNumber { get; set; }
        public int IntentosFallidosNumber { get; set; }
    }
}