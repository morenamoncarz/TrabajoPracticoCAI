namespace Orders.API.Models;

// Representa una orden realizada por un usuario.
public class Order
{
    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }

    // Lista de productos incluidos en la orden
    public List<OrderItem> Items { get; set; } = new();

    // Total calculado en base a cantidad y precio unitario
    public decimal Total { get; set; }

    // Estado inicial: Pendiente
    public string Estado { get; set; } = "Pendiente";

    public DateTime FechaCreacion { get; set; }
}