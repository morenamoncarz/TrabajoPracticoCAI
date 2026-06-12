using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly OrdersApiClient _ordersApiClient;
    private readonly CartApiClient _cartApiClient;
    private readonly NotificationsApiClient _notificationsApiClient;
    private readonly IConfiguration _config;

    public ProductService(
        IProductRepository repo,
        OrdersApiClient ordersApiClient,
        CartApiClient cartApiClient,
        NotificationsApiClient notificationsApiClient,
        IConfiguration config)
    {
        _repo = repo;
        _ordersApiClient = ordersApiClient;
        _cartApiClient = cartApiClient;
        _notificationsApiClient = notificationsApiClient;
        _config = config;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(string? categoria = null, string? nombre = null)
    {
        return await _repo.GetAllAsync(categoria, nombre);
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        var producto = await _repo.GetByIdAsync(id);
        if (producto == null)
        {
            throw new NotFoundException("PRD-001", "Producto no encontrado.");
        }
        return producto;
    }

    public async Task<Product> CreateAsync(Product producto)
    {
        if (await _repo.ExisteAsync(producto.Nombre, producto.Categoria))
        {
            throw new BusinessRuleException("PRD-003", $"Ya existe un producto con ese nombre en la categoria '{producto.Categoria}'.");
        }
        producto.Id = Guid.NewGuid();
        producto.FechaCreacion = DateTime.UtcNow;
        await _repo.AddAsync(producto);
        return producto;
    }

    public async Task<Product> UpdateAsync(Guid id, Product producto)
    {
        var existente = await _repo.GetByIdAsync(id);
        if (existente == null)
        {
            throw new NotFoundException("PRD-001", "Producto no encontrado.");
        }
        producto.Id = id;
        producto.FechaCreacion = existente.FechaCreacion;
        await _repo.UpdateAsync(producto);

        await NotificarCambiosAlCarrito(existente, producto);

        return producto;
    }

    // avisa a quienes tienen el producto en el carrito si cambio el precio o se esta agotando
    private async Task NotificarCambiosAlCarrito(Product viejo, Product nuevo)
    {
        var umbral = _config.GetValue<int?>("Notifications:StockBajoUmbral") ?? 5;

        var mensajes = new List<string>();

        if (nuevo.Precio != viejo.Precio)
            mensajes.Add($"El producto {nuevo.Nombre} de tu carrito cambió de precio: ahora ${nuevo.Precio}.");

        if (viejo.Stock > 0 && nuevo.Stock == 0)
            mensajes.Add($"El producto {nuevo.Nombre} de tu carrito se quedó sin stock.");
        else if (viejo.Stock > umbral && nuevo.Stock <= umbral)
            mensajes.Add($"El producto {nuevo.Nombre} de tu carrito está por agotarse (quedan {nuevo.Stock}).");

        if (mensajes.Count == 0)
            return;

        var usuarios = await _cartApiClient.GetUsuariosConProductoEnCarrito(nuevo.Id);

        foreach (var usuarioId in usuarios)
            foreach (var mensaje in mensajes)
                await _notificationsApiClient.Notificar(usuarioId, mensaje);
    }

    public async Task DeleteAsync(Guid id)
    {
        var producto = await _repo.GetByIdAsync(id);
        if (producto == null)
        {
            throw new NotFoundException("PRD-001", "Producto no encontrado.");
        }

        // no lo dejo borrar si esta en alguna orden activa
        if (await _ordersApiClient.ProductoTieneOrdenesActivas(id))
        {
            throw new BusinessRuleException(
                "PRD-004",
                "El producto tiene ordenes activas y no puede eliminarse.");
        }

        await _repo.DeleteAsync(id);
    }
}