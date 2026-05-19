using Serilog;
using Serilog.Formatting.Compact;

namespace Cart.API.Extensions;

public static class LoggingExtensions
{
    public static void AddAppLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Servicio", "Cart.API")
            .WriteTo.Console()
            .WriteTo.File(new CompactJsonFormatter(), "logs/cart-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}
