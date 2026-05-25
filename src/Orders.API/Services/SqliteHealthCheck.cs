using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Orders.API.Services;

public class SqliteHealthCheck : IHealthCheck
{
    private readonly IConfiguration _config;

    public SqliteHealthCheck(IConfiguration config)
    {
        _config = config;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Tomamos la conexión desde appsettings.json
            var connectionString =
                _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=orders.db";

            using var connection = new SqliteConnection(connectionString);

            connection.Open();

            // Si pudimos abrir conexión, SQLite está funcionando
            return Task.FromResult(
                HealthCheckResult.Healthy(
                    "SQLite funcionando correctamente."));
        }
        catch (Exception ex)
        {
            // Si falla la conexión, devolvemos unhealthy
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "Error conectando SQLite.",
                    ex));
        }
    }
}