using System.ComponentModel.DataAnnotations;

namespace Orders.API.DTOs;

public class UpdateOrderStatusRequest
{
    // Nuevo estado de la orden
    [Required]
    public string Estado { get; set; } = "";
}