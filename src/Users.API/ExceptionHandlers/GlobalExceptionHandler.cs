using Microsoft.AspNetCore.Diagnostics;

namespace Users.API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Maneja cualquier error no controlado
        context.Response.StatusCode = 500;

        // Tomamos el correlation id generado por el middleware
        var correlationId = context.Items["X-Correlation-Id"]?.ToString();

        await context.Response.WriteAsJsonAsync(new
        {
            type = "about:blank",
            title = "Error interno",
            status = 500,
            detail = "Ocurrió un error inesperado.",
            instance = context.Request.Path.Value,
            errorCode = "USR-006",
            errorMessage = "Error interno al procesar el usuario.",
            correlationId = correlationId
        }, cancellationToken);

        return true;
    }
}