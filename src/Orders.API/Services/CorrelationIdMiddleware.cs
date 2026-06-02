using Serilog.Context;

namespace Orders.API.Services;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Si el request ya trae correlation id, lo usamos.
        // Si no trae, generamos uno nuevo.
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Lo agregamos al response para poder rastrear la operación.
        context.Response.Headers[HeaderName] = correlationId;

        // Lo dejamos disponible para los handlers de errores.
        context.Items[HeaderName] = correlationId;

        // Lo agregamos al contexto de Serilog.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}