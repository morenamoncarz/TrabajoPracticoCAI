namespace Cliente.Consola.Menus;

public static class ProductosMenu
{
    public static async Task Mostrar()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- PRODUCTOS ---");
            Console.WriteLine("1) listar");
            Console.WriteLine("2) ver detalle");
            Console.WriteLine("3) crear");
            Console.WriteLine("4) editar");
            Console.WriteLine("5) borrar");
            Console.WriteLine("6) volver");
            Console.Write("opcion: ");

            switch (Console.ReadLine())
            {
                case "1": await Listar(); break;
                case "2": await Ver(); break;
                case "3": await Crear(); break;
                case "4": await Editar(); break;
                case "5": await Borrar(); break;
                case "6": return;
                default: Console.WriteLine("opcion invalida"); break;
            }
        }
    }

    private static async Task Listar()
    {
        var productos = await ApiClient.Get<List<ProductoDto>>($"{ApiUrls.Products}/api/products");
        if (productos == null) return;
        if (productos.Count == 0) { Console.WriteLine("no hay productos"); return; }

        for (var i = 0; i < productos.Count; i++)
            Console.WriteLine($"{i + 1}) {productos[i].Nombre} ${productos[i].Precio} stock:{productos[i].Stock}");
    }

    // muestra la lista numerada y devuelve el producto elegido, asi no hay que tipear uuids
    private static async Task<ProductoDto?> Elegir()
    {
        var productos = await ApiClient.Get<List<ProductoDto>>($"{ApiUrls.Products}/api/products");
        if (productos == null) return null;
        if (productos.Count == 0) { Console.WriteLine("no hay productos"); return null; }

        for (var i = 0; i < productos.Count; i++)
            Console.WriteLine($"{i + 1}) {productos[i].Nombre} ${productos[i].Precio} stock:{productos[i].Stock}");

        Console.Write("numero del producto: ");
        if (!int.TryParse(Console.ReadLine(), out var n) || n < 1 || n > productos.Count)
        {
            Console.WriteLine("numero invalido");
            return null;
        }
        return productos[n - 1];
    }

    private static async Task Ver()
    {
        var elegido = await Elegir();
        if (elegido == null) return;

        var p = await ApiClient.Get<ProductoDto>($"{ApiUrls.Products}/api/products/{elegido.Id}");
        if (p != null)
            Console.WriteLine($"{p.Nombre} - {p.Descripcion} - ${p.Precio} - stock {p.Stock} - {p.Categoria}");
    }

    private static async Task Crear()
    {
        Console.Write("nombre: "); var nombre = Console.ReadLine();
        Console.Write("descripcion: "); var descripcion = Console.ReadLine();
        Console.Write("precio: "); decimal.TryParse(Console.ReadLine(), out var precio);
        Console.Write("stock: "); int.TryParse(Console.ReadLine(), out var stock);
        Console.Write("categoria: "); var categoria = Console.ReadLine();

        var body = new { nombre, descripcion, precio, stock, categoria };
        var creado = await ApiClient.Post<ProductoDto>($"{ApiUrls.Products}/api/products", body);
        if (creado != null)
            Console.WriteLine($"producto creado: {creado.Nombre}");
    }

    private static async Task Editar()
    {
        var elegido = await Elegir();
        if (elegido == null) return;

        Console.Write("nombre: "); var nombre = Console.ReadLine();
        Console.Write("descripcion: "); var descripcion = Console.ReadLine();
        Console.Write("precio: "); decimal.TryParse(Console.ReadLine(), out var precio);
        Console.Write("stock: "); int.TryParse(Console.ReadLine(), out var stock);
        Console.Write("categoria: "); var categoria = Console.ReadLine();

        var body = new { nombre, descripcion, precio, stock, categoria };
        var editado = await ApiClient.Put<ProductoDto>($"{ApiUrls.Products}/api/products/{elegido.Id}", body);
        if (editado != null)
            Console.WriteLine("producto actualizado");
    }

    private static async Task Borrar()
    {
        var elegido = await Elegir();
        if (elegido == null) return;

        var ok = await ApiClient.Delete($"{ApiUrls.Products}/api/products/{elegido.Id}");
        if (ok) Console.WriteLine("producto borrado");
    }
}

public class ProductoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string Categoria { get; set; } = "";
}
