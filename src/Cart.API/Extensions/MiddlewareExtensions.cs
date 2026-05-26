using Cart.API.Middleware;
using Serilog;
using Serilog.Events;

namespace Cart.API.Extensions;

public static class MiddlewareExtensions
{
    public static void UseAppMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (httpContext, _, ex) =>
                (ex != null) ? LogEventLevel.Error :
                (httpContext.Request.Path.StartsWithSegments("/health"))
                    ? LogEventLevel.Verbose : LogEventLevel.Information;
        });
        app.UseMiddleware<AuditMiddleware>();
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
    }
}
