using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MinimalApi.Interfaces;
using MinimalApi.Services;

namespace MinimalApi.CustomPolicy;

public class TokenBasedRateLimiter : IRateLimiterPolicy<string>
{
    private readonly ILogger<TokenBasedRateLimiter> _logger;

    public TokenBasedRateLimiter(ILogger<TokenBasedRateLimiter> logger)
    {
        _logger = logger;
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected =>
        (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return new ValueTask(context.HttpContext.Response.WriteAsync("Rate limit exceeded or insufficient tokens.", token));
        };

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var username = httpContext.User.Identity?.Name ?? "anonymous";
        _logger.LogInformation($"Creating rate limit partition for user: {username}");

        return RateLimitPartition.GetTokenBucketLimiter(username, options =>
        {
            var tokenService = httpContext.RequestServices.GetRequiredService<ITokenService>();
           // var tokenService = _serviceProvider.GetRequiredService<ITokenService>();
            var canConsume = tokenService.ConsumeTokenAsync(username).GetAwaiter().GetResult();
            if (!canConsume)
            {
                throw new InvalidOperationException("Insufficient tokens");
            }

            return new TokenBucketRateLimiterOptions
            {
                TokenLimit = 1,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                TokensPerPeriod = 1,
                AutoReplenishment = false
            };
        });
    }


}