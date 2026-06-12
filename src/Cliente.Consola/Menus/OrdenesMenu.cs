namespace Cliente.Consola.Menus;

public static class OrdenesMenu
{
    public static async Task Mostrar()
    {
        if (Sesion.UsuarioActualId == null)
        {
            Console.WriteLine("primero logueate en el menu de usuarios");
            return;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- ORDENES ---");
            Console.WriteLine("1) listar todas");
            Console.WriteLine("2) ver detalle");
            Console.WriteLine("3) crear");
            Console.WriteLine("4) cambiar estado");
            Console.WriteLine("5) volver");
            Console.Write("opcion: ");

            switch (Console.ReadLine())
            {
                case "1": await Listar(); break;
                case "2": await Ver(); break;
                case "3": await Crear(); break;
                case "4": await CambiarEstado(); break;
                case "5": return;
                default: Console.WriteLine("opcion invalida"); break;
            }
        }
    }

    private static async Task Listar()
    {
        var ordenes = await ApiClient.Get<List<OrdenDto>>($"{ApiUrls.Orders}/api/orders");
        if (ordenes == null) return;
        if (ordenes.Count == 0) { Console.WriteLine("no hay ordenes"); return; }

        for (var i = 0; i < ordenes.Count; i++)
            Console.WriteLine($"{i + 1}) total ${ordenes[i].Total} estado:{ordenes[i].Estado}");
    }

    private static async Task<OrdenDto?> Elegir()
    {
        var ordenes = await ApiClient.Get<List<OrdenDto>>($"{ApiUrls.Orders}/api/orders");
        if (ordenes == null) return null;
        if (ordenes.Count == 0) { Console.WriteLine("no hay ordenes"); return null; }

        for (var i = 0; i < ordenes.Count; i++)
            Console.WriteLine($"{i + 1}) total ${ordenes[i].Total} estado:{ordenes[i].Estado}");

        Console.Write("numero de la orden: ");
        if (!int.TryParse(Console.ReadLine(), out var n) || n < 1 || n > ordenes.Count)
        {
            Console.WriteLine("numero invalido");
            return null;
        }
        return ordenes[n - 1];
    }

    private static async Task Ver()
    {
        var elegida = await Elegir();
        if (elegida == null) return;

        var o = await ApiClient.Get<OrdenDto>($"{ApiUrls.Orders}/api/orders/{elegida.Id}");
        if (o == null) return;

        // traigo los nombres para no mostrar uuids en los items
        var productos = await ApiClient.Get<List<ProductoDto>>($"{ApiUrls.Products}/api/products");
        var nombres = productos?.ToDictionary(p => p.Id, p => p.Nombre) ?? new();

        Console.WriteLine($"orden de ${o.Total} - estado {o.Estado}");
        foreach (var i in o.Items)
            Console.WriteLine($"  {nombres.GetValueOrDefault(i.ProductoId, "producto desconocido")} x{i.Cantidad} a ${i.PrecioUnitario}");
    }

    private static async Task Crear()
    {
        var productos = await ApiClient.Get<List<ProductoDto>>($"{ApiUrls.Products}/api/products");
        if (productos == null) return;
        if (productos.Count == 0) { Console.WriteLine("no hay productos"); return; }

        for (var i = 0; i < productos.Count; i++)
            Console.WriteLine($"{i + 1}) {productos[i].Nombre} ${productos[i].Precio} stock:{productos[i].Stock}");

        var items = new List<object>();
        while (true)
        {
            Console.Write("numero del producto (enter para terminar): ");
            var linea = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(linea)) break;

            if (!int.TryParse(linea, out var n) || n < 1 || n > productos.Count)
            {
                Console.WriteLine("numero invalido");
                continue;
            }

            Console.Write("cantidad: "); int.TryParse(Console.ReadLine(), out var cantidad);
            items.Add(new { productoId = productos[n - 1].Id, cantidad });
        }

        var body = new { usuarioId = Sesion.UsuarioActualId, items };
        var orden = await ApiClient.Post<OrdenDto>($"{ApiUrls.Orders}/api/orders", body);
        if (orden != null) Console.WriteLine($"orden creada, total ${orden.Total}");
    }

    private static async Task CambiarEstado()
    {
        var elegida = await Elegir();
        if (elegida == null) return;

        Console.Write("nuevo estado (Confirmada/Enviada/Entregada/Cancelada): ");
        var estado = Console.ReadLine();

        var body = new { estado };
        var orden = await ApiClient.Put<OrdenDto>($"{ApiUrls.Orders}/api/orders/{elegida.Id}/status", body);
        if (orden != null) Console.WriteLine($"estado cambiado a {orden.Estado}");
    }
}

public class OrdenDto
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "";
    public List<OrdenItemDto> Items { get; set; } = new();
}

public class OrdenItemDto
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
