using Dapper;
using Microsoft.Data.Sqlite;
using Orders.API.Models;

namespace Orders.API.Services;

public class OrderRepositoryDb : IOrderRepository
{
    private readonly IConfiguration _config;

    public OrderRepositoryDb(IConfiguration config)
    {
        _config = config;
    }

    // Crea la conexión a SQLite usando appsettings.json
    private SqliteConnection CreateConnection()
    {
        var connectionString =
            _config.GetConnectionString("DefaultConnection")
            ?? "Data Source=orders.db";

        return new SqliteConnection(connectionString);
    }

    // Devuelve todas las órdenes
    // Si usuarioId tiene valor, filtra por usuario
    public List<Order> GetAll(Guid? usuarioId)
    {
        using var connection = CreateConnection();

        var orders = connection.Query<OrderDbRow>("""
            SELECT
                id,
                usuario_id,
                total,
                estado,
                fecha_creacion
            FROM orders
        """).ToList();

        var result = new List<Order>();

        foreach (var row in orders)
        {
            var order = MapOrder(row, connection);

            // Filtramos por usuario solo si se recibió usuarioId
            if (usuarioId == null || order.UsuarioId == usuarioId)
            {
                result.Add(order);
            }
        }

        return result;
    }

    // Busca una orden por id
    public Order? GetById(Guid id)
    {
        using var connection = CreateConnection();

        var row = connection.QueryFirstOrDefault<OrderDbRow>("""
            SELECT
                id,
                usuario_id,
                total,
                estado,
                fecha_creacion
            FROM orders
            WHERE id = @Id
        """, new
        {
            Id = id.ToString()
        });

        if (row == null)
        {
            return null;
        }

        return MapOrder(row, connection);
    }

    // Guarda una nueva orden y sus items
    public void Add(Order order)
    {
        using var connection = CreateConnection();

        // Insertamos cabecera de la orden
        connection.Execute("""
            INSERT INTO orders (
                id,
                usuario_id,
                total,
                estado,
                fecha_creacion
            )
            VALUES (
                @Id,
                @UsuarioId,
                @Total,
                @Estado,
                @FechaCreacion
            )
        """, new
        {
            Id = order.Id.ToString(),
            UsuarioId = order.UsuarioId.ToString(),
            order.Total,
            order.Estado,
            FechaCreacion = order.FechaCreacion.ToString("O")
        });

        // Insertamos cada item asociado a la orden
        foreach (var item in order.Items)
        {
            connection.Execute("""
                INSERT INTO order_items (
                    order_id,
                    producto_id,
                    cantidad,
                    precio_unitario
                )
                VALUES (
                    @OrderId,
                    @ProductoId,
                    @Cantidad,
                    @PrecioUnitario
                )
            """, new
            {
                OrderId = order.Id.ToString(),
                ProductoId = item.ProductoId.ToString(),
                item.Cantidad,
                item.PrecioUnitario
            });
        }
    }

    // Actualiza el estado de una orden
    public void UpdateStatus(Guid id, string estado)
    {
        using var connection = CreateConnection();

        connection.Execute("""
            UPDATE orders
            SET estado = @Estado
            WHERE id = @Id
        """, new
        {
            Id = id.ToString(),
            Estado = estado
        });
    }

    // Convierte los datos de SQLite a un objeto Order
    private Order MapOrder(
        OrderDbRow row,
        SqliteConnection connection)
    {
        // Buscamos los items de la orden
        var items = connection.Query<OrderItemDbRow>("""
            SELECT
                producto_id,
                cantidad,
                precio_unitario
            FROM order_items
            WHERE order_id = @OrderId
        """, new
        {
            OrderId = row.Id
        });

        return new Order
        {
            Id = Guid.Parse(row.Id),
            UsuarioId = Guid.Parse(row.UsuarioId),
            Total = row.Total,
            Estado = row.Estado,
            FechaCreacion = DateTime.Parse(row.FechaCreacion),

            Items = items.Select(i => new OrderItem
            {
                ProductoId = Guid.Parse(i.ProductoId),
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            }).ToList()
        };
    }

    // Clase auxiliar para leer datos de SQLite
    private class OrderDbRow
    {
        public string Id { get; set; } = "";
        public string UsuarioId { get; set; } = "";
        public decimal Total { get; set; }
        public string Estado { get; set; } = "";
        public string FechaCreacion { get; set; } = "";
    }

    // Clase auxiliar para leer items desde SQLite
    private class OrderItemDbRow
    {
        public string ProductoId { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}