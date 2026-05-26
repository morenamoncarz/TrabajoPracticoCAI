using Cart.API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Cart.API.ExceptionHandlers;

public class BusinessRuleExceptionHandler : IExceptionHandler
{
    private readonly ILogger<BusinessRuleExceptionHandler> _logger;

    public BusinessRuleExceptionHandler(ILogger<BusinessRuleExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BusinessRuleException ex) return false;

        _logger.LogWarning("Regla de negocio violada. ErrorCode: {ErrorCode}", ex.ErrorCode);

        var correlationId = context.Response.Headers["X-Correlation-Id"].ToString();
        context.Response.StatusCode = 422;
        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            title = "Unprocessable Entity",
            status = 422,
            detail = "No se puede procesar la solicitud.",
            instance = context.Request.Path.Value,
            correlationId,
            errorCode = ex.ErrorCode,
            errorMessage = ex.Message
        }, cancellationToken: cancellationToken);

        return true;
    }
}
