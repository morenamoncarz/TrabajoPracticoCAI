using Serilog;
using Serilog.Formatting.Compact;

namespace Products.API.Extensions;

public static class LoggingExtensions
{
    public static void AddAppLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Servicio", "Products.API")
            .WriteTo.Console()
            .WriteTo.File(new CompactJsonFormatter(), "logs/products-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}
