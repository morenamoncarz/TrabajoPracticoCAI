using Microsoft.AspNetCore.Diagnostics;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers;

public class NotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Solo manejamos NotFoundException
        if (exception is not NotFoundException ex)
            return false;

        context.Response.StatusCode = 404;

        // Tomamos el correlation id generado por el middleware
        var correlationId = context.Items["X-Correlation-Id"]?.ToString();

        await context.Response.WriteAsJsonAsync(new
        {
            type = "about:blank",
            title = "Not Found",
            status = 404,
            detail = ex.Message,
            instance = context.Request.Path.Value,
            errorCode = ex.ErrorCode,
            errorMessage = ex.Message,
            correlationId = correlationId
        }, cancellationToken);

        return true;
    }
}