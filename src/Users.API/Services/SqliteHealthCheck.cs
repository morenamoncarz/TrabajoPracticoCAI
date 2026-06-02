using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Users.API.Services;

public class SqliteHealthCheck : IHealthCheck
{
    private readonly IConfiguration _config;

    public SqliteHealthCheck(IConfiguration config)
    {
        _config = config;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Tomamos la conexión configurada en appsettings.json
            var connectionString =
                _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=users.db";

            // Abrimos conexión a SQLite
            using var connection = new SqliteConnection(connectionString);

            await connection.OpenAsync(cancellationToken);

            // Consulta mínima para validar que la base responde
            await connection.ExecuteScalarAsync<int>("SELECT 1");

            return HealthCheckResult.Healthy("SQLite responde correctamente.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "No se pudo conectar a SQLite.",
                ex
            );
        }
    }
}