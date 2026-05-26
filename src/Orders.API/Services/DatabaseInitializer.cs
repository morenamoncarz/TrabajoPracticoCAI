using Dapper;
using Microsoft.Data.Sqlite;

namespace Orders.API.Services;

public class DatabaseInitializer
{
    private readonly IConfiguration _config;

    public DatabaseInitializer(IConfiguration config)
    {
        _config = config;
    }

    public void Initialize()
    {
        // Tomamos la conexión desde appsettings.json
        var connectionString =
            _config.GetConnectionString("DefaultConnection")
            ?? "Data Source=orders.db";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        // Tabla principal de órdenes
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS orders (
                id TEXT PRIMARY KEY,
                usuario_id TEXT NOT NULL,
                total REAL NOT NULL,
                estado TEXT NOT NULL,
                fecha_creacion TEXT NOT NULL
            );
        """);

        // Tabla de items de cada orden
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS order_items (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id TEXT NOT NULL,
                producto_id TEXT NOT NULL,
                cantidad INTEGER NOT NULL,
                precio_unitario REAL NOT NULL,
                FOREIGN KEY(order_id) REFERENCES orders(id)
            );
        """);
    }
}