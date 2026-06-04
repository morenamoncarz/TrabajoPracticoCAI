using System.Net.Http.Json;
using System.Text.Json;

namespace Cliente.Consola;

public static class ApiClient
{
    private static readonly HttpClient _http = new HttpClient();

    private static readonly JsonSerializerOptions _opciones = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T?> Get<T>(string url)
    {
        return await Enviar<T>(HttpMethod.Get, url, null);
    }

    public static async Task<T?> Post<T>(string url, object body)
    {
        return await Enviar<T>(HttpMethod.Post, url, body);
    }

    public static async Task<T?> Put<T>(string url, object body)
    {
        return await Enviar<T>(HttpMethod.Put, url, body);
    }

    public static async Task<bool> Delete(string url)
    {
        try
        {
            var resp = await _http.DeleteAsync(url);
            if (!resp.IsSuccessStatusCode)
            {
                await MostrarError(resp);
                return false;
            }
            return true;
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("no me puedo conectar a la api, esta levantada?");
            return false;
        }
    }

    private static async Task<T?> Enviar<T>(HttpMethod metodo, string url, object? body)
    {
        try
        {
            var pedido = new HttpRequestMessage(metodo, url);
            if (body != null)
            {
                pedido.Content = JsonContent.Create(body);
            }

            var resp = await _http.SendAsync(pedido);

            if (!resp.IsSuccessStatusCode)
            {
                await MostrarError(resp);
                return default;
            }

            var json = await resp.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, _opciones);
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("no me puedo conectar a la api, esta levantada?");
            return default;
        }
    }

    private static async Task MostrarError(HttpResponseMessage resp)
    {
        var json = await resp.Content.ReadAsStringAsync();
        try
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(json, _opciones);
            if (error != null && !string.IsNullOrEmpty(error.ErrorCode))
            {
                Console.WriteLine($"error {error.ErrorCode}: {error.ErrorMessage}");
                return;
            }
        }
        catch { }

        Console.WriteLine($"hubo un error ({(int)resp.StatusCode})");
    }
}

public class ErrorResponse
{
    public string ErrorCode { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
}
