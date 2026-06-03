using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cart.API.HealthChecks;

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
            var cs = _config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db";
            using var conn = new SqliteConnection(cs);
            await conn.OpenAsync(cancellationToken);
            await conn.ExecuteScalarAsync<int>("SELECT 1");
            return HealthCheckResult.Healthy("sqlite ok");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("no se pudo conectar a sqlite", ex);
        }
    }
}
