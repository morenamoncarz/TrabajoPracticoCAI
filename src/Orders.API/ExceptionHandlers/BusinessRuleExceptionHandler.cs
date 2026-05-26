using Microsoft.AspNetCore.Diagnostics;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

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

        // Definimos el HTTP status según el catálogo ORD
        int statusCode = ex.ErrorCode switch
        {
            "ORD-002" => 400, // datos inválidos
            "ORD-005" => 422, // stock insuficiente
            "ORD-006" => 409, // transición de estado inválida
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