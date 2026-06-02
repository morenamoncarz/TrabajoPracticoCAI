namespace Cart.API.Http;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
    public decimal Precio { get; set; }
}
