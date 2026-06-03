namespace Notifications.API.Services;

// cliente http para preguntarle a users api si el usuario existe
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

    public async Task<bool> UserExists(Guid usuarioId)
    {
        var baseUrl =
            _config["Services:UsersApi"]
            ?? "http://localhost:5029";

        // si responde 200 existe, si responde 404 no
        var response = await _httpClient.GetAsync(
            $"{baseUrl}/api/users/{usuarioId}");

        return response.IsSuccessStatusCode;
    }
}
