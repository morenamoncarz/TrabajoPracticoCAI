using System.ComponentModel.DataAnnotations;

namespace Products.API.Models;

public class Product
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = "";

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    public string Categoria { get; set; } = "";

    public DateTime FechaCreacion { get; set; }
}