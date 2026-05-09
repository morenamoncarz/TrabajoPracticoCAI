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

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapeamos los controllers (esto activa /api/users)
app.MapControllers();

app.Run();