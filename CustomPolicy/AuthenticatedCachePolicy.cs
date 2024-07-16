using Microsoft.AspNetCore.OutputCaching;

namespace MinimalApi.CustomPolicy;

public class AuthenticatedCachePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;

        // Vary cache by authentication status
        //context.CacheVaryByRules.VaryByValues=isAuthenticated ? "Authenticated" : "Anonymous");
        //context.CacheVaryByRules.QueryKeys = new string[] { isAuthenticated ? "auth" : "noauth" };

        context.Tags.Add("Religion");
        context.AllowCacheLookup = true;
        context.AllowCacheStorage = true;
        context.AllowLocking = true;

        // Log cache decision
        Console.WriteLine($"Cache decision for {context.HttpContext.Request.Path}: Lookup: {context.AllowCacheLookup}, Storage: {context.AllowCacheStorage}, Auth: {isAuthenticated}");

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Serving from cache: {context.HttpContext.Request.Path}");
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Serving response: {context.HttpContext.Request.Path}");
        return ValueTask.CompletedTask;
    }
}
