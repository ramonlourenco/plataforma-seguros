using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Contratacao.Infrastructure.Clients;

public sealed class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public const string HeaderName = "X-Correlation-ID";

    public CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = _httpContextAccessor.HttpContext?.Request.Headers[HeaderName].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            if (request.Headers.Contains(HeaderName))
            {
                request.Headers.Remove(HeaderName);
            }
            request.Headers.Add(HeaderName, correlationId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
