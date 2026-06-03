namespace Products.API.Services;

public class CorrelationIdPropagationHandler : DelegatingHandler
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly IHttpContextAccessor _accessor;

    public CorrelationIdPropagationHandler(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var ctx = _accessor.HttpContext;
        if (ctx is not null &&
            ctx.Response.Headers.TryGetValue(HeaderName, out var value) &&
            !string.IsNullOrWhiteSpace(value))
        {
            if (!request.Headers.Contains(HeaderName))
                request.Headers.TryAddWithoutValidation(HeaderName, value.ToString());
        }
        return base.SendAsync(request, ct);
    }
}
