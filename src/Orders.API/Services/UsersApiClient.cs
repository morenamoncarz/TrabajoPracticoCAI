namespace Orders.API.Services;

// Cliente HTTP para consultar Users.API
public class UsersApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public UsersApiClient(
        HttpClient httpClient,
        IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    // Verifica si un usuario existe consultando Users.API
    public async Task<bool> UserExists(Guid usuarioId)
    {
        // URL base definida en appsettings.json
        var baseUrl =
            _config["Services:UsersApi"]
            ?? "http://localhost:5000";

        // Users.API expone GET /api/users/{id}
        // Si responde 200, el usuario existe.
        // Si responde 404, el usuario no existe.
        var response = await _httpClient.GetAsync(
            $"{baseUrl}/api/users/{usuarioId}");

        return response.IsSuccessStatusCode;
    }
}