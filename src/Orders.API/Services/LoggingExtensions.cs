using Serilog;
using Serilog.Events;

namespace Orders.API.Services;

public static class LoggingExtensions
{
    public static void AddAppLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()

            // Nivel mínimo general de logs
            .MinimumLevel.Information()

            // Bajamos ruido de logs internos de Microsoft
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)

            // Agregamos información del contexto, como CorrelationId
            .Enrich.FromLogContext()

            // Consola: mostramos errores importantes
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Error
            )

            // Archivo: guardamos logs de la API
            .WriteTo.File(
                path: "logs/orders-api-.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information
            )

            .CreateLogger();

        // Usamos Serilog como sistema principal de logs
        builder.Host.UseSerilog();
    }
}