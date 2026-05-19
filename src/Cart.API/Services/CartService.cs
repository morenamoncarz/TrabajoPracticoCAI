using Cart.API.Data;
using Cart.API.Exceptions;
using Cart.API.Models;

namespace Cart.API.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repo;

    public CartService(ICartRepository repo)
    {
        _repo = repo;
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

        // TODO sprint 8: validar via IProductsClient que el producto existe (CRT-002) y stock alcanza (CRT-003)

        var existente = await _repo.GetCantidadAsync(usuarioId, request.ProductoId);
        var nuevaCantidad = (existente ?? 0) + request.Cantidad;

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

        // TODO sprint 8: validar stock contra Products API (CRT-003)

        await _repo.AddOrUpdateItemAsync(usuarioId, productoId, request.Cantidad);
        return (await _repo.GetByUsuarioAsync(usuarioId))!;
    }

    public async Task RemoveItemAsync(Guid usuarioId, Guid productoId)
    {
        var cart = await _repo.GetByUsuarioAsync(usuarioId);
        if (cart is null)
            throw new NotFoundException("CRT-001", "Carrito no encontrado.");

        // DELETE idempotente: si el item ya no estaba, igual devolvemos 204
        await _repo.RemoveItemAsync(usuarioId, productoId);
    }

    public async Task ClearCartAsync(Guid usuarioId)
    {
        var borrado = await _repo.ClearCartAsync(usuarioId);
        if (!borrado)
            throw new NotFoundException("CRT-001", "Carrito no encontrado.");
    }
}
