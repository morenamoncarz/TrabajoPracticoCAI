using System.ComponentModel.DataAnnotations;

namespace Orders.API.DTOs;

public class CreateOrderRequest
{
    // Usuario que realiza la orden
    [Required]
    public Guid UsuarioId { get; set; }

    // Lista de productos de la orden
    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}