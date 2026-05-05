using Products.API.Models;

namespace Products.API.Services;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(string? categoria = null, string? nombre = null);
    Task<Product?> GetByIdAsync(Guid id);
    Task AddAsync(Product producto);
    Task<bool> UpdateAsync(Product producto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExisteAsync(string nombre, string categoria);
}