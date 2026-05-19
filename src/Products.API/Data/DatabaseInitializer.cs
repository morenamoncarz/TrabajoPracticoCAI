using Dapper;
using Microsoft.Data.Sqlite;

namespace Products.API.Data;

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
        var cs = _config.GetConnectionString("DefaultConnection") ?? "Data Source=products.db";

        using var conn = new SqliteConnection(cs);
        conn.Open();

        conn.Execute("""
            CREATE TABLE IF NOT EXISTS products (
                id            TEXT PRIMARY KEY,
                nombre        TEXT NOT NULL COLLATE NOCASE,
                descripcion   TEXT,
                precio        REAL NOT NULL,
                stock         INTEGER NOT NULL,
                categoria     TEXT NOT NULL COLLATE NOCASE,
                fechaCreacion TEXT NOT NULL
            );
        """);

        _logger.LogInformation("sqlite inicializado en {db}", cs);
    }
}
