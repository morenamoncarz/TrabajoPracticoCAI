using Serilog;
using Serilog.Events;

namespace Users.API.Services;

public static class LoggingExtensions
{
    public static void AddAppLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()

            // Nivel mínimo global
            .MinimumLevel.Information()

            // Reducimos ruido de logs internos de Microsoft
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)

            // Agregamos contexto automático
            .Enrich.FromLogContext()

            // Logs de errores en consola
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Error
            )

            // Logs HTTP en archivo
            .WriteTo.File(
                path: "logs/users-api-.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information
            )

            .CreateLogger();

        // Reemplazamos el sistema de logs de ASP.NET por Serilog
        builder.Host.UseSerilog();
    }
}