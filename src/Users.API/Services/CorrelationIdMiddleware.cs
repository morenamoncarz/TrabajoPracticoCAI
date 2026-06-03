using Serilog.Context;

namespace Users.API.Services;

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

        // Lo agregamos al response para que el cliente también lo pueda ver.
        context.Response.Headers[HeaderName] = correlationId;

        // Lo guardamos en HttpContext para que otros componentes puedan usarlo.
        context.Items[HeaderName] = correlationId;

        // Lo agregamos al contexto de Serilog para que aparezca en los logs.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}