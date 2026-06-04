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
            Console.WriteLine("2) ver por id");
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
        if (ordenes != null)
            foreach (var o in ordenes)
                Console.WriteLine($"- orden {o.Id.ToString().Substring(0, 4)}.. total ${o.Total} estado:{o.Estado}");
    }

    private static async Task Ver()
    {
        Console.Write("id de la orden: "); var id = Console.ReadLine();
        var o = await ApiClient.Get<OrdenDto>($"{ApiUrls.Orders}/api/orders/{id}");
        if (o != null)
        {
            Console.WriteLine($"orden {o.Id} - total ${o.Total} - estado {o.Estado}");
            foreach (var i in o.Items)
                Console.WriteLine($"  producto {i.ProductoId.ToString().Substring(0, 4)}.. x{i.Cantidad} a ${i.PrecioUnitario}");
        }
    }

    private static async Task Crear()
    {
        var items = new List<object>();
        while (true)
        {
            Console.Write("id del producto (enter para terminar): ");
            var productoId = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(productoId)) break;
            Console.Write("cantidad: "); int.TryParse(Console.ReadLine(), out var cantidad);
            items.Add(new { productoId, cantidad });
        }

        var body = new { usuarioId = Sesion.UsuarioActualId, items };
        var orden = await ApiClient.Post<OrdenDto>($"{ApiUrls.Orders}/api/orders", body);
        if (orden != null) Console.WriteLine($"orden creada con id {orden.Id}, total ${orden.Total}");
    }

    private static async Task CambiarEstado()
    {
        Console.Write("id de la orden: "); var id = Console.ReadLine();
        Console.Write("nuevo estado (Confirmada/Enviada/Entregada/Cancelada): ");
        var estado = Console.ReadLine();

        // mando el estado pegado en la url
        var orden = await ApiClient.Put<OrdenDto>($"{ApiUrls.Orders}/api/orders/{id}/status?estado={estado}", new { });
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
