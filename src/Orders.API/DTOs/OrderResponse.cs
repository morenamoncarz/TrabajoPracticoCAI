namespace Orders.API.DTOs;

public class OrderResponse
{
    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }

    // Items incluidos en la orden
    public List<OrderItemResponse> Items { get; set; } = new();

    public decimal Total { get; set; }

    public string Estado { get; set; } = "";

    public DateTime FechaCreacion { get; set; }
}