using Dapper;
using Microsoft.Data.Sqlite;
using Products.API.Models;
using Products.API.Services;

namespace Products.API.Data;

public class ProductRepository : IProductRepository
{
    private readonly IConfiguration _config;

    public ProductRepository(IConfiguration config)
    {
        _config = config;
    }

    private SqliteConnection CreateConnection() =>
        new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=products.db");

    public async Task<IEnumerable<Product>> GetAllAsync(string? categoria = null, string? nombre = null)
    {
        using var conn = CreateConnection();
        var sql = "SELECT id AS Id, nombre AS Nombre, descripcion AS Descripcion, precio AS Precio, stock AS Stock, categoria AS Categoria, fechaCreacion AS FechaCreacion FROM products WHERE 1=1";
        var parametros = new DynamicParameters();
        if (!string.IsNullOrWhiteSpace(categoria))
        {
            sql += " AND categoria = @Categoria";
            parametros.Add("Categoria", categoria);
        }
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            sql += " AND nombre LIKE @Nombre";
            parametros.Add("Nombre", $"%{nombre}%");
        }
        return await conn.QueryAsync<Product>(sql, parametros);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        using var conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Product>(
            "SELECT id AS Id, nombre AS Nombre, descripcion AS Descripcion, precio AS Precio, stock AS Stock, categoria AS Categoria, fechaCreacion AS FechaCreacion FROM products WHERE id = @Id",
            new { Id = id.ToString() });
    }

    public async Task AddAsync(Product producto)
    {
        using var conn = CreateConnection();
        var sql = $"INSERT INTO products (id, nombre, descripcion, precio, stock, categoria, fechaCreacion) " +
                  $"VALUES ('{producto.Id}', '{producto.Nombre}', '{producto.Descripcion}', {producto.Precio}, {producto.Stock}, '{producto.Categoria}', '{producto.FechaCreacion:o}')";
        await conn.ExecuteAsync(sql);
    }

    public async Task<bool> UpdateAsync(Product producto)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync(
            "UPDATE products SET nombre = @Nombre, descripcion = @Descripcion, precio = @Precio, stock = @Stock, categoria = @Categoria WHERE id = @Id",
            new
            {
                Id = producto.Id.ToString(),
                producto.Nombre,
                producto.Descripcion,
                producto.Precio,
                producto.Stock,
                producto.Categoria
            });
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync(
            "DELETE FROM products WHERE id = @Id",
            new { Id = id.ToString() });
        return rows > 0;
    }

    public async Task<bool> ExisteAsync(string nombre, string categoria)
    {
        using var conn = CreateConnection();
        var count = await conn.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM products WHERE nombre = @Nombre AND categoria = @Categoria",
            new { Nombre = nombre, Categoria = categoria });
        return count > 0;
    }
}
