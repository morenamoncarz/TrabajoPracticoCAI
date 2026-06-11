namespace Cliente.Consola.Menus;

public static class NotificacionesMenu
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
            Console.WriteLine("--- NOTIFICACIONES ---");
            Console.WriteLine("1) ver las mias");
            Console.WriteLine("2) volver");
            Console.Write("opcion: ");

            switch (Console.ReadLine())
            {
                case "1": await Ver(); break;
                case "2": return;
                default: Console.WriteLine("opcion invalida"); break;
            }
        }
    }

    private static async Task Ver()
    {
        var notifs = await ApiClient.Get<List<NotificacionDto>>($"{ApiUrls.Notifications}/api/notifications/{Sesion.UsuarioActualId}");
        if (notifs != null)
            foreach (var n in notifs)
                Console.WriteLine($"- [{n.Tipo}] {n.Mensaje} ({n.Estado})");
    }
}

public class NotificacionDto
{
    public Guid Id { get; set; }
    public string Mensaje { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Estado { get; set; } = "";
}
