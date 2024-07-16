using MinimalApi.Interfaces;

namespace MinimalApi.CustomMiddleware;
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TokenBasedRateLimitingAttribute : Attribute
{
}

public class ConditionalTokenConsumptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ConditionalTokenConsumptionMiddleware> _logger;

    public ConditionalTokenConsumptionMiddleware(RequestDelegate next, ILogger<ConditionalTokenConsumptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        Endpoint? endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var tokenBasedRateLimiting = endpoint.Metadata.GetMetadata<TokenBasedRateLimitingAttribute>();
            if (tokenBasedRateLimiting != null)
            {
                var username = context.User.Identity?.Name ?? "anonymous";

                if (username != "anonymous")
                {
                    var canConsume = await tokenService.ConsumeTokenAsync(username);
                    if (!canConsume)
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsync("Rate limit exceeded or insufficient tokens.");
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}


