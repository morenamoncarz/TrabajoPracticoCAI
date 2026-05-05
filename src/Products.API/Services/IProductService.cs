using Products.API.Models;

namespace Products.API.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync(string? categoria = null, string? nombre = null);
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> CreateAsync(Product producto);
    Task<Product?> UpdateAsync(Guid id, Product producto);
    Task<bool> DeleteAsync(Guid id);
}