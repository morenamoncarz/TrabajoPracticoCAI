using Dapper;
using Microsoft.Data.Sqlite;

namespace Users.API.Services;

public class DatabaseInitializer
{
    private readonly IConfiguration _config;

    public DatabaseInitializer(IConfiguration config)
    {
        _config = config;
    }

    public void Initialize()
    {
        // Tomamos la cadena de conexión desde appsettings.json.
        // Si no existe, usamos users.db como valor por defecto.
        var connectionString =
            _config.GetConnectionString("DefaultConnection")
            ?? "Data Source=users.db";

        // Abrimos la conexión con SQLite.
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        // Creamos la tabla de usuarios si todavía no existe.
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS users (
                id TEXT PRIMARY KEY,
                nombre TEXT NOT NULL,
                apellido TEXT NOT NULL,
                email TEXT NOT NULL UNIQUE,
                password_hash TEXT NOT NULL,
                fecha_registro TEXT NOT NULL,
                activo INTEGER NOT NULL,
                intentos_fallidos INTEGER NOT NULL
            );
        """);
    }
}