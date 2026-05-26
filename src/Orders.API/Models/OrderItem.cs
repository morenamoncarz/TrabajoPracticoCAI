namespace Orders.API.Models;

// Representa un producto dentro de una orden.
public class OrderItem
{
    public Guid ProductoId { get; set; }

    public int Cantidad { get; set; }

    // Precio del producto al momento de crear la orden
    public decimal PrecioUnitario { get; set; }
}