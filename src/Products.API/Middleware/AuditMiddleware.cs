using System.Text;
using System.Text.Json;

namespace Products.API.Middleware;

public class AuditMiddleware
{
    private static readonly HashSet<string> MetodosAuditados =
        new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "DELETE" };

    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!MetodosAuditados.Contains(context.Request.Method) ||
            context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();
        var requestBody = await LeerBodyAsync(context.Request.Body);
        context.Request.Body.Position = 0;

        var responseOriginal = context.Response.Body;
        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await _next(context);

        buffer.Position = 0;
        var responseBody = await new StreamReader(buffer).ReadToEndAsync();
        buffer.Position = 0;
        await buffer.CopyToAsync(responseOriginal);
        context.Response.Body = responseOriginal;

        _logger.LogInformation(
            "AUDIT {Method} {Path} {StatusCode} {@RequestBody} {@ResponseBody}",
            context.Request.Method,
            context.Request.Path.Value,
            context.Response.StatusCode,
            ParsearJson(requestBody),
            ParsearJson(responseBody));
    }

    private static async Task<string> LeerBodyAsync(Stream body)
    {
        using var reader = new StreamReader(body, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private static object? ParsearJson(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        try { return JsonSerializer.Deserialize<object>(raw); }
        catch { return raw; }
    }
}
