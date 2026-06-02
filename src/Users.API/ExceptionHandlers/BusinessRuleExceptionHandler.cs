using Microsoft.AspNetCore.Diagnostics;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers;

public class BusinessRuleExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Solo manejamos BusinessRuleException
        if (exception is not BusinessRuleException ex)
            return false;

        // Definimos status según errorCode
        int statusCode = ex.ErrorCode switch
        {
            "USR-001" => 409, // email duplicado
            "USR-003" => 401, // credenciales incorrectas
            "USR-004" => 403, // usuario bloqueado
            _ => 400
        };

        context.Response.StatusCode = statusCode;

        // Tomamos el correlation id generado por el middleware
        var correlationId = context.Items["X-Correlation-Id"]?.ToString();

        await context.Response.WriteAsJsonAsync(new
        {
            type = "about:blank",
            title = "Error de negocio",
            status = statusCode,
            detail = ex.Message,
            instance = context.Request.Path.Value,
            errorCode = ex.ErrorCode,
            errorMessage = ex.Message,
            correlationId = correlationId
        }, cancellationToken);

        return true;
    }
}