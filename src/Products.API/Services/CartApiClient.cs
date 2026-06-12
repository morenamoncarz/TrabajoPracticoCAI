using System.Net.Http.Json;

namespace Products.API.Services;

// le pregunta a cart que usuarios tienen un producto agregado, para avisarles de cambios
public class CartApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<CartApiClient> _logger;

    public CartApiClient(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<CartApiClient> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    // usuarios que tienen el producto en su carrito (vacio si cart esta caido o nadie lo tiene)
    public async Task<List<Guid>> GetUsuariosConProductoEnCarrito(Guid productoId)
    {
        var baseUrl = _config["Services:CartApi"] ?? "http://localhost:5277";

        try
        {
            var usuarios = await _httpClient.GetFromJsonAsync<List<Guid>>(
                $"{baseUrl}/api/cart/with-product/{productoId}");

            return usuarios ?? new List<Guid>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "fallo al consultar cart por el producto {ProductoId}", productoId);
            return new List<Guid>();
        }
    }
}
