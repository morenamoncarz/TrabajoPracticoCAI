using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notifications.API.ExceptionHandlers;
using Notifications.API.Middleware;
using Notifications.API.Services;
using Notifications.API.Data;
using Notifications.API.Repositories;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Servicio", "Notifications.API")
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(), "logs/notifications-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// cliente http hacia users api para validar ntf-001
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdPropagationHandler>();
builder.Services.AddHttpClient<UsersApiClient>()
    .AddHttpMessageHandler<CorrelationIdPropagationHandler>();

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "ready", "live" });

builder.Services.Configure<ApiBehaviorOptions>(options =>
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
            errorCode = "NTF-002",
            errorMessage = errores
        })
        {
            StatusCode = 400
        };
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider
        .GetRequiredService<DatabaseInitializer>()
        .Initialize();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapControllers();

static Task EscribirHealthCheck(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var respuesta = new
    {
        estado = report.Status.ToString(),
        checks = report.Entries.Select(e => new
        {
            nombre = e.Key,
            estado = e.Value.Status.ToString()
        })
    };

    return context.Response.WriteAsJsonAsync(respuesta);
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = EscribirHealthCheck
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = EscribirHealthCheck
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = EscribirHealthCheck
});

app.Run();