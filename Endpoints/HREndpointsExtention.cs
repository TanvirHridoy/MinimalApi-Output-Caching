using MinimalApi.Endpoints.GroupExtensions;

namespace MinimalApi.Endpoints;

public static class HREndpointsExtention
{
    public static void MapHREndpoints(this WebApplication app)
    {
        //app.MapGroup("/api/MasterData").MapMasterDataHrGroup().RequireAuthorization().WithOpenApi();
        app.MapGroup("/api/employees").MapHREmployeeGroup().RequireAuthorization().WithOpenApi();
        app.MapGroup("/api/religion").MapHRReligionGroup().WithOpenApi().RequireAuthorization()
            .CacheOutput("ReligionPolicy");
    }
}
