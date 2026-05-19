using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Products.API.Extensions;

public static class EndpointsExtensions
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapControllers();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = EscribirHealthCheck
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = EscribirHealthCheck
        });
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = EscribirHealthCheck
        });
    }

    private static Task EscribirHealthCheck(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        var respuesta = new
        {
            estado = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                nombre = e.Key,
                estado = e.Value.Status.ToString()
            })
        };
        return context.Response.WriteAsJsonAsync(respuesta);
    }
}
