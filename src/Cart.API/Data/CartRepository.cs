using Cart.API.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Cart.API.Data;

public class CartRepository : ICartRepository
{
    private readonly IConfiguration _config;

    public CartRepository(IConfiguration config)
    {
        _config = config;
    }

    private SqliteConnection CreateConnection()
    {
        var conn = new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db");
        conn.Open();
        // sqlite trae las FK apagadas por default, hay que prenderlas en cada conexion
        conn.Execute("PRAGMA foreign_keys = ON;");
        return conn;
    }

    public async Task<Models.Cart?> GetByUsuarioAsync(Guid usuarioId)
    {
        using var conn = CreateConnection();
        var fila = await conn.QuerySingleOrDefaultAsync<(string UsuarioId, string FechaActualizacion)?>(
            "SELECT usuarioId, fechaActualizacion FROM carts WHERE usuarioId = @UsuarioId",
            new { UsuarioId = usuarioId.ToString() });

        if (fila is null) return null;

        var items = (await conn.QueryAsync<(string ProductoId, int Cantidad)>(
            "SELECT productoId, cantidad FROM cart_items WHERE usuarioId = @UsuarioId",
            new { UsuarioId = usuarioId.ToString() })).ToList();

        return new Models.Cart
        {
            UsuarioId = usuarioId,
            FechaActualizacion = DateTime.Parse(fila.Value.FechaActualizacion).ToUniversalTime(),
            Items = items.Select(i => new CartItem
            {
                ProductoId = Guid.Parse(i.ProductoId),
                Cantidad = i.Cantidad
            }).ToList()
        };
    }

    public async Task AddOrUpdateItemAsync(Guid usuarioId, Guid productoId, int cantidad)
    {
        using var conn = CreateConnection();
        var ahora = DateTime.UtcNow.ToString("o");

        await conn.ExecuteAsync(
            """
            INSERT INTO carts (usuarioId, fechaActualizacion)
            VALUES (@UsuarioId, @Fecha)
            ON CONFLICT(usuarioId) DO UPDATE SET fechaActualizacion = excluded.fechaActualizacion;

            INSERT INTO cart_items (usuarioId, productoId, cantidad)
            VALUES (@UsuarioId, @ProductoId, @Cantidad)
            ON CONFLICT(usuarioId, productoId) DO UPDATE SET cantidad = excluded.cantidad;
            """,
            new
            {
                UsuarioId = usuarioId.ToString(),
                ProductoId = productoId.ToString(),
                Cantidad = cantidad,
                Fecha = ahora
            });
    }

    public async Task<bool> RemoveItemAsync(Guid usuarioId, Guid productoId)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync(
            "DELETE FROM cart_items WHERE usuarioId = @UsuarioId AND productoId = @ProductoId",
            new { UsuarioId = usuarioId.ToString(), ProductoId = productoId.ToString() });

        if (rows > 0)
        {
            await conn.ExecuteAsync(
                "UPDATE carts SET fechaActualizacion = @Fecha WHERE usuarioId = @UsuarioId",
                new { UsuarioId = usuarioId.ToString(), Fecha = DateTime.UtcNow.ToString("o") });
        }
        return rows > 0;
    }

    public async Task<bool> ClearCartAsync(Guid usuarioId)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync(
            "DELETE FROM carts WHERE usuarioId = @UsuarioId",
            new { UsuarioId = usuarioId.ToString() });
        return rows > 0;
    }

    public async Task<bool> ItemExisteAsync(Guid usuarioId, Guid productoId)
    {
        using var conn = CreateConnection();
        var count = await conn.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM cart_items WHERE usuarioId = @UsuarioId AND productoId = @ProductoId",
            new { UsuarioId = usuarioId.ToString(), ProductoId = productoId.ToString() });
        return count > 0;
    }

    public async Task<int?> GetCantidadAsync(Guid usuarioId, Guid productoId)
    {
        using var conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<int?>(
            "SELECT cantidad FROM cart_items WHERE usuarioId = @UsuarioId AND productoId = @ProductoId",
            new { UsuarioId = usuarioId.ToString(), ProductoId = productoId.ToString() });
    }

    public async Task<List<Guid>> GetUsuariosConProductoAsync(Guid productoId)
    {
        using var conn = CreateConnection();
        var ids = await conn.QueryAsync<string>(
            "SELECT DISTINCT usuarioId FROM cart_items WHERE productoId = @ProductoId",
            new { ProductoId = productoId.ToString() });

        return ids.Select(Guid.Parse).ToList();
    }
}
