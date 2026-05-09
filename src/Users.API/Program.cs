using Users.API.ExceptionHandlers;
using Users.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregamos soporte para controllers (esto permite usar UsersController)
builder.Services.AddControllers();

// Swagger (documentación de la API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registramos nuestras dependencias
// Cuando alguien pida IUserRepository, usamos UserRepositoryMemory
builder.Services.AddSingleton<IUserRepository, UserRepositoryMemory>();

// Registramos el servicio donde está la lógica del negocio
builder.Services.AddScoped<UserService>();

// Registramos los manejadores globales de errores
// Esto permite devolver errorCode y errorMessage en vez de un error genérico
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Habilitamos Swagger para poder probar la API desde el navegador
app.UseSwagger();
app.UseSwaggerUI();

// Activamos el manejo global de errores
app.UseExceptionHandler();

app.UseHttpsRedirection();

// Mapeamos los controllers (esto activa /api/users)
app.MapControllers();

app.Run();