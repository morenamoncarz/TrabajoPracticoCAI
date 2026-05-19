using Cart.API.Data;
using Cart.API.ExceptionHandlers;
using Cart.API.HealthChecks;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Extensions;

public static class ServicesExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);
        });

        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<BusinessRuleExceptionHandler>();
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddSingleton<DatabaseInitializer>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartService, CartService>();

        services.AddHealthChecks()
            .AddCheck<SqliteHealthCheck>("sqlite-db", tags: new[] { "database", "ready" })
            .AddCheck<ApiStatusCheck>("api-status", tags: new[] { "api" });

        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(600);
            setup.AddHealthCheckEndpoint("Cart.API", "/health");
        }).AddInMemoryStorage();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = ctx =>
            {
                var errores = string.Join("; ", ctx.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                var correlationId = ctx.HttpContext.Response.Headers["X-Correlation-Id"].ToString();
                return new ObjectResult(new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = "Bad Request",
                    status = 400,
                    detail = "Los datos son invalidos.",
                    instance = ctx.HttpContext.Request.Path.Value,
                    correlationId,
                    errorCode = "CRT-004",
                    errorMessage = errores
                })
                {
                    StatusCode = 400
                };
            };
        });
    }
}
