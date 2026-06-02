using System.Text.Json;

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

    // Obtiene un producto real desde Products.API
    public async Task<ProductInfo?> GetProductAsync(Guid productoId)
    {
        // URL base definida en appsettings.json
        var baseUrl =
            _config["Services:ProductsApi"]
            ?? "http://localhost:5290";

        var response = await _httpClient.GetAsync(
            $"{baseUrl}/api/products/{productoId}");

        // Si el producto no existe devolvemos null
        if (response.StatusCode ==
            System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        // Si hubo otro error HTTP lanzamos excepción
        response.EnsureSuccessStatusCode();

        // Convertimos el JSON del Products.API a ProductInfo
        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ProductInfo>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
}

// Modelo auxiliar para leer la respuesta del Products.API
public class ProductInfo
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = "";

    public decimal Precio { get; set; }

    public int Stock { get; set; }
}