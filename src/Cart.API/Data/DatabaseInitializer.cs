using Dapper;
using Microsoft.Data.Sqlite;

namespace Cart.API.Data;

public class DatabaseInitializer
{
    private readonly IConfiguration _config;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration config, ILogger<DatabaseInitializer> logger)
    {
        _config = config;
        _logger = logger;
    }

    public void Initialize()
    {
        var cs = _config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db";

        using var conn = new SqliteConnection(cs);
        conn.Open();

        conn.Execute("""
            CREATE TABLE IF NOT EXISTS carts (
                usuarioId          TEXT PRIMARY KEY,
                fechaActualizacion TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS cart_items (
                usuarioId  TEXT NOT NULL,
                productoId TEXT NOT NULL,
                cantidad   INTEGER NOT NULL,
                PRIMARY KEY (usuarioId, productoId),
                FOREIGN KEY (usuarioId) REFERENCES carts(usuarioId) ON DELETE CASCADE
            );
        """);

        _logger.LogInformation("sqlite inicializado en {db}", cs);
    }
}
