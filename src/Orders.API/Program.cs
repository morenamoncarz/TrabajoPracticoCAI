using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.API.ExceptionHandlers;
using Orders.API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuramos Serilog para logs en consola y archivo
builder.AddAppLogging();

// Agregamos soporte para controllers
builder.Services.AddControllers();

// Personalizamos errores de validación (ORD-002)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errores = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
            .ToList();

        var mensaje = errores.Any()
            ? string.Join("; ", errores)
            : "Los datos de la orden son inválidos.";

        return new BadRequestObjectResult(new
        {
            type = "about:blank",
            title = "Datos inválidos",
            status = 400,
            detail = mensaje,
            instance = context.HttpContext.Request.Path.Value,
            errorCode = "ORD-002",
            errorMessage = "Los datos de la orden son inválidos.",
            correlationId = context.HttpContext.Items["X-Correlation-Id"]?.ToString()
        });
    };
});

// Swagger con XML comments
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile =
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath =
        Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
});

// Base de datos SQLite + Dapper
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<IOrderRepository, OrderRepositoryDb>();

// Servicios
builder.Services.AddScoped<OrderService>();

// Necesario para leer el request actual y propagar X-Correlation-Id
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdPropagationHandler>();

// Clientes HTTP hacia otros microservicios
builder.Services.AddHttpClient<UsersApiClient>()
    .AddHttpMessageHandler<CorrelationIdPropagationHandler>();

builder.Services.AddHttpClient<ProductsApiClient>()
    .AddHttpMessageHandler<CorrelationIdPropagationHandler>();

// Exception handlers
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck<SqliteHealthCheck>("sqlite-db")
    .AddCheck("api-status", () =>
        HealthCheckResult.Healthy("API funcionando correctamente."));

var app = builder.Build();

// Inicializamos la base de datos
using (var scope = app.Services.CreateScope())
{
    var databaseInitializer =
        scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

    databaseInitializer.Initialize();
}

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Manejo global de errores
app.UseExceptionHandler();

// Correlation ID
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseHttpsRedirection();

// Logging automático de requests HTTP
app.UseSerilogRequestLogging();

// Health checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

// Controllers
app.MapControllers();

app.Run();