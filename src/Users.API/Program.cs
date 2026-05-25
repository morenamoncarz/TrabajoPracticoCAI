using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Users.API.ExceptionHandlers;
using Users.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuramos Serilog para logs en consola y archivo
builder.AddAppLogging();

// Agregamos soporte para controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de datos
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<IUserRepository, UserRepositoryDb>();

// Lógica de negocio
builder.Services.AddScoped<UserService>();

// Manejo global de errores
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
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

app.UseHttpsRedirection();

// Log automático de requests HTTP
app.UseSerilogRequestLogging();

// Health checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

// Controllers
app.MapControllers();

app.Run();