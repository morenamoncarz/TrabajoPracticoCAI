using System.Net.Http.Json;

namespace Cart.API.Http;

// le avisa a notifications api cuando pasa algo con el carrito
public class NotificationsClient : INotificationsClient
{
    private readonly HttpClient _http;
    private readonly ILogger<NotificationsClient> _logger;

    public NotificationsClient(HttpClient http, ILogger<NotificationsClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task Notificar(Guid usuarioId, string mensaje, CancellationToken ct = default)
    {
        var body = new { usuarioId, mensaje, tipo = "Push" };

        // si notifications esta caido el carrito igual tiene que andar, asi que no propago el error
        try
        {
            var response = await _http.PostAsJsonAsync("/api/notifications/send", body, ct);

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
