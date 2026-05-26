using Microsoft.AspNetCore.Mvc;
using Products.API.Data;
using Products.API.ExceptionHandlers;
using Products.API.HealthChecks;
using Products.API.Services;

namespace Products.API.Extensions;

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
            c.IncludeXmlComments(xmlPath);
        });

        services.AddSingleton<DatabaseInitializer>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<BusinessRuleExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddHealthChecks()
            .AddCheck<SqliteHealthCheck>("sqlite-db", tags: new[] { "database", "ready" })
            .AddCheck<ApiStatusCheck>("api-status", tags: new[] { "api" });

        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(600);
            setup.AddHealthCheckEndpoint("Products.API", "/health");
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
                    errorCode = "PRD-002",
                    errorMessage = errores
                })
                {
                    StatusCode = 400
                };
            };
        });
    }
}
