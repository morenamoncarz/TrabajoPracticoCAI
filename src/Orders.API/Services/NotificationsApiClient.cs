using System.Net.Http.Json;

namespace Orders.API.Services;

// cliente http para avisarle a notifications api cuando pasa algo con una orden
public class NotificationsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public NotificationsApiClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task Notificar(Guid usuarioId, string mensaje)
    {
        var baseUrl =
            _config["Services:NotificationsApi"]
            ?? "http://localhost:5026";

        var body = new { usuarioId, mensaje, tipo = "Push" };

        var response = await _httpClient.PostAsJsonAsync(
            $"{baseUrl}/api/notifications/send", body);

        response.EnsureSuccessStatusCode();
    }
}
