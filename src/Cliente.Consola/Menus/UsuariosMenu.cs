namespace Cliente.Consola.Menus;

public static class UsuariosMenu
{
    public static async Task Mostrar()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- USUARIOS ---");
            Console.WriteLine("1) registrarme");
            Console.WriteLine("2) login");
            Console.WriteLine("3) ver mis datos");
            Console.WriteLine("4) volver");
            Console.Write("opcion: ");

            switch (Console.ReadLine())
            {
                case "1": await Registrar(); break;
                case "2": await Login(); break;
                case "3": await VerDatos(); break;
                case "4": return;
                default: Console.WriteLine("opcion invalida"); break;
            }
        }
    }

    private static async Task Registrar()
    {
        Console.Write("nombre: "); var nombre = Console.ReadLine();
        Console.Write("apellido: "); var apellido = Console.ReadLine();
        Console.Write("email: "); var email = Console.ReadLine();
        Console.Write("password: "); var password = Console.ReadLine();

        var body = new { nombre, apellido, email, password };
        var user = await ApiClient.Post<UsuarioDto>($"{ApiUrls.Users}/api/users/register", body);

        if (user != null)
        {
            Sesion.UsuarioActualId = user.Id;
            Sesion.NombreUsuario = user.Nombre;
            Console.WriteLine($"listo, te registraste y quedaste logueado como {user.Nombre}");
        }
    }

    private static async Task Login()
    {
        Console.Write("email: "); var email = Console.ReadLine();
        Console.Write("password: "); var password = Console.ReadLine();

        var body = new { email, password };
        var user = await ApiClient.Post<UsuarioDto>($"{ApiUrls.Users}/api/users/login", body);

        if (user != null)
        {
            Sesion.UsuarioActualId = user.Id;
            Sesion.NombreUsuario = user.Nombre;
            Console.WriteLine($"hola {user.Nombre}, entraste bien");
        }
    }

    private static async Task VerDatos()
    {
        if (Sesion.UsuarioActualId == null)
        {
            Console.WriteLine("primero logueate");
            return;
        }

        var user = await ApiClient.Get<UsuarioDto>($"{ApiUrls.Users}/api/users/{Sesion.UsuarioActualId}");
        if (user != null)
            Console.WriteLine($"{user.Nombre} {user.Apellido} - {user.Email}");
    }
}

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
}
