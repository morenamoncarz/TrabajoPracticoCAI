using Microsoft.AspNetCore.Diagnostics;

namespace Products.API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error inesperado");
        var correlationId = context.Response.Headers["X-Correlation-Id"].ToString();
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "Internal Server Error",
            status = 500,
            detail = "Ocurrio un error inesperado.",
            instance = context.Request.Path.Value,
            correlationId,
            errorCode = "PRD-005",
            errorMessage = "Error interno al procesar el producto."
        }, cancellationToken: cancellationToken);
        return true;
    }
}