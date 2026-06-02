using System.Text.Json;

namespace Products.API.Services;

// le pregunta a orders si el producto esta en alguna orden activa
public class OrdersApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public OrdersApiClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    // true si hay alguna orden pendiente o confirmada con ese producto
    public async Task<bool> ProductoTieneOrdenesActivas(Guid productoId)
    {
        var baseUrl = _config["Services:OrdersApi"] ?? "http://localhost:5074";

        var response = await _httpClient.GetAsync($"{baseUrl}/api/orders");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var ordenes = JsonSerializer.Deserialize<List<OrdenInfo>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new List<OrdenInfo>();

        var estadosActivos = new[] { "Pendiente", "Confirmada" };

        return ordenes.Any(o =>
            estadosActivos.Contains(o.Estado) &&
            o.Items.Any(i => i.ProductoId == productoId));
    }

    // para leer lo que devuelve orders
    private class OrdenInfo
    {
        public string Estado { get; set; } = "";

        public List<ItemInfo> Items { get; set; } = new();
    }

    private class ItemInfo
    {
        public Guid ProductoId { get; set; }
    }
}
