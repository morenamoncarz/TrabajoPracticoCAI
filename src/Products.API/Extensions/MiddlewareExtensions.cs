using Products.API.Middleware;

namespace Products.API.Extensions;

public static class MiddlewareExtensions
{
    public static void UseAppMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
    }
}
