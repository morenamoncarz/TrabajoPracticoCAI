using System.Net.Http.Json;

namespace Orders.API.Services;

// cliente http para avisarle a notifications api cuando pasa algo con una orden
public class NotificationsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<NotificationsApiClient> _logger;

    public NotificationsApiClient(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<NotificationsApiClient> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task Notificar(Guid usuarioId, string mensaje)
    {
        var baseUrl =
            _config["Services:NotificationsApi"]
            ?? "http://localhost:5026";

        var body = new { usuarioId, mensaje, tipo = "Push" };

        // si notifications esta caido la orden igual tiene que andar, asi que no propago el error
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{baseUrl}/api/notifications/send", body);

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning(
                    "no se pudo notificar al usuario {UsuarioId}, status {Status}",
                    usuarioId, (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "fallo al notificar al usuario {UsuarioId}", usuarioId);
        }
    }
}
