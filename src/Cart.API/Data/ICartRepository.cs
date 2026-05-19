using Cart.API.Models;

namespace Cart.API.Data;

public interface ICartRepository
{
    Task<Models.Cart?> GetByUsuarioAsync(Guid usuarioId);
    Task AddOrUpdateItemAsync(Guid usuarioId, Guid productoId, int cantidad);
    Task<bool> RemoveItemAsync(Guid usuarioId, Guid productoId);
    Task<bool> ClearCartAsync(Guid usuarioId);
    Task<bool> ItemExisteAsync(Guid usuarioId, Guid productoId);
    Task<int?> GetCantidadAsync(Guid usuarioId, Guid productoId);
}
