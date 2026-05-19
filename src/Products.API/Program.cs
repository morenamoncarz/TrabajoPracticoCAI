using Products.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppLogging();
builder.Services.AddAppServices();

var app = builder.Build();

app.UseAppMiddleware();
app.MapAppEndpoints();

app.Run();
