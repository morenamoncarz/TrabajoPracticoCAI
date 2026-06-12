using Cart.API.Models;

namespace Cart.API.Services;

public interface ICartService
{
    Task<Models.Cart> GetCartAsync(Guid usuarioId);
    Task<Models.Cart> AddItemAsync(Guid usuarioId, AddCartItemRequest request);
    Task<Models.Cart> UpdateItemAsync(Guid usuarioId, Guid productoId, UpdateCartItemRequest request);
    Task RemoveItemAsync(Guid usuarioId, Guid productoId);
    Task ClearCartAsync(Guid usuarioId);
    Task<List<Guid>> GetUsuariosConProductoAsync(Guid productoId);
}
