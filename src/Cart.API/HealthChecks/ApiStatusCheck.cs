using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cart.API.HealthChecks;

public class ApiStatusCheck : IHealthCheck
{
    private static readonly DateTime Inicio = DateTime.UtcNow;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var uptime = DateTime.UtcNow - Inicio;
        var data = new Dictionary<string, object>
        {
            ["runtime"] = $".NET {Environment.Version}",
            ["uptime"] = uptime.ToString(@"hh\:mm\:ss"),
            ["startedAt"] = Inicio.ToString("o")
        };

        return Task.FromResult(HealthCheckResult.Healthy("api operativa", data));
    }
}
