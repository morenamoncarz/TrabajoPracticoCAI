using System.ComponentModel.DataAnnotations;

namespace Products.API.Models;

/// <summary>Producto del catalogo.</summary>
public class Product
{
    public Guid Id { get; set; }

    /// <summary>Nombre del producto.</summary>
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = "";

    /// <summary>Descripcion opcional.</summary>
    [StringLength(500)]
    public string? Descripcion { get; set; }

    /// <summary>Precio unitario, mayor a 0.</summary>
    [Range(0.01, double.MaxValue)]
    public decimal Precio { get; set; }

    /// <summary>Stock disponible, mayor o igual a 0.</summary>
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    /// <summary>Categoria del producto.</summary>
    [Required]
    public string Categoria { get; set; } = "";

    public DateTime FechaCreacion { get; set; }
}