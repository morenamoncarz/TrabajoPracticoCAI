namespace Orders.API.Services;

// Cliente HTTP para consultar Products.API
public class ProductsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public ProductsApiClient(
        HttpClient httpClient,
        IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    // Simula obtener precio de producto
    // Más adelante puede conectarse al Products.API real
    public async Task<decimal> GetProductPrice(Guid productoId)
    {
        var baseUrl = _config["Services:ProductsApi"];

        var response = await _httpClient.GetAsync(
            $"{baseUrl}/api/products/{productoId}");

        // Si Products.API no está levantado,
        // devolvemos un precio dummy temporal
        if (!response.IsSuccessStatusCode)
        {
            return 1000;
        }

        // Por ahora dejamos un valor fijo simple
        return 1000;
    }

    // Simula validar stock disponible
    public async Task<bool> HasStock(
        Guid productoId,
        int cantidad)
    {
        var baseUrl = _config["Services:ProductsApi"];

        var response = await _httpClient.GetAsync(
            $"{baseUrl}/api/products/{productoId}");

        // Temporalmente asumimos que hay stock
        return response.IsSuccessStatusCode || true;
    }
}