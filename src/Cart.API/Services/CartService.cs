using Cart.API.Data;
using Cart.API.Exceptions;
using Cart.API.Http;
using Cart.API.Models;

namespace Cart.API.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repo;
    private readonly IProductsClient _products;

    public CartService(ICartRepository repo, IProductsClient products)
    {
        _repo = repo;
        _products = products;
    }

    public async Task<Models.Cart> GetCartAsync(Guid usuarioId)
    {
        var cart = await _repo.GetByUsuarioAsync(usuarioId);
        if (cart is null)
            throw new NotFoundException("CRT-001", "Carrito no encontrado.");
        return cart;
    }

    public async Task<Models.Cart> AddItemAsync(Guid usuarioId, AddCartItemRequest request)
    {
        if (request.Cantidad <= 0)
            throw new ValidationException("CRT-004", "Cantidad invalida.");

        var producto = await _products.GetByIdAsync(request.ProductoId);
        if (producto is null)
            throw new NotFoundException("CRT-002", "Producto no encontrado.");

        var existente = await _repo.GetCantidadAsync(usuarioId, request.ProductoId);
        var nuevaCantidad = (existente ?? 0) + request.Cantidad;

        if (nuevaCantidad > producto.Stock)
            throw new BusinessRuleException("CRT-003",
                $"Stock insuficiente. Disponible: {producto.Stock}, solicitado: {nuevaCantidad}.");

        await _repo.AddOrUpdateItemAsync(usuarioId, request.ProductoId, nuevaCantidad);
        return (await _repo.GetByUsuarioAsync(usuarioId))!;
    }

    public async Task<Models.Cart> UpdateItemAsync(Guid usuarioId, Guid productoId, UpdateCartItemRequest request)
    {
        if (request.Cantidad <= 0)
            throw new ValidationException("CRT-004", "Cantidad invalida.");

        var cart = await _repo.GetByUsuarioAsync(usuarioId);
        if (cart is null)
            throw new NotFoundException("CRT-001", "Carrito no encontrado.");

        if (!await _repo.ItemExisteAsync(usuarioId, productoId))
            throw new NotFoundException("CRT-001", "Item no encontrado en el carrito.");

        var producto = await _products.GetByIdAsync(productoId);
        if (producto is null)
            throw new NotFoundException("CRT-002", "Producto no encontrado.");

        if (request.Cantidad > producto.Stock)
            throw new BusinessRuleException("CRT-003",
                $"Stock insuficiente. Disponible: {producto.Stock}, solicitado: {request.Cantidad}.");

        await _repo.AddOrUpdateItemAsync(usuarioId, productoId, request.Cantidad);
        return (await _repo.GetByUsuarioAsync(usuarioId))!;
    }

    public async Task RemoveItemAsync(Guid usuarioId, Guid productoId)
    {
        var cart = await _repo.GetByUsuarioAsync(usuarioId);
        if (cart is null)
            throw new NotFoundException("CRT-001", "Carrito no encontrado.");

        await _repo.RemoveItemAsync(usuarioId, productoId);
    }

    public async Task ClearCartAsync(Guid usuarioId)
    {
        var borrado = await _repo.ClearCartAsync(usuarioId);
        if (!borrado)
            throw new NotFoundException("CRT-001", "Carrito no encontrado.");
    }
}
