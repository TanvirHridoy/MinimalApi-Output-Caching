using MinimalApi.Endpoints.GroupExtensions;

namespace MinimalApi.Endpoints;

public static class AuthEndpointsExtension
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
       
        app.MapGroup("/api/Auth").MapAuthGroup().RequireAuthorization().WithOpenApi();
    }
}
