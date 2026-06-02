namespace Notifications.API.Services;

// pasa el x-correlation-id a las llamadas que salen hacia otras apis
public class CorrelationIdPropagationHandler : DelegatingHandler
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly IHttpContextAccessor _accessor;

    public CorrelationIdPropagationHandler(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var context = _accessor.HttpContext;

        if (context is not null &&
            context.Response.Headers.TryGetValue(HeaderName, out var correlationId) &&
            !string.IsNullOrWhiteSpace(correlationId))
        {
            if (!request.Headers.Contains(HeaderName))
            {
                request.Headers.TryAddWithoutValidation(
                    HeaderName,
                    correlationId.ToString());
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
