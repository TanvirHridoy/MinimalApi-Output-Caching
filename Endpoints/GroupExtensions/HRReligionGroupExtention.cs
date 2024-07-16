using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApi.CustomMiddleware;
using MinimalApi.DTO;
using MinimalApi.Extensions;
using MinimalApi.Interfaces;
using MinimalApi.Repository;

namespace MinimalApi.Endpoints.GroupExtensions;

public static class HRReligionGroupExtention
{
    public static RouteGroupBuilder MapHRReligionGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/GetAllReligion", async (HttpContext httpContext, [FromServices] CustomApiMetrics customMetrics, [FromServices] ReligionRepository repo, [FromServices] ITokenService tokenService) =>
        {
            var religions = await repo.GetAll();
            return Results.Ok(religions);
        }).WithName("GetAllReligion");

        group.MapGet("/GetReligionById/{id}", async ([FromServices] ReligionRepository repo, int id) =>
        {
            var religion = await repo.GetById(id);
            return religion != null ? Results.Ok(religion) : Results.NotFound();
        }).WithName("GetReligionById");

        group.MapPost("/CreateReligion", async (IOutputCacheStore cache,  [FromServices] ReligionRepository repo, Religion religion, CancellationToken ct) =>
        {
            var NewReligion = await repo.Add(religion);
            await cache.EvictByTagAsync("Religion",ct);
            return Results.CreatedAtRoute("GetReligionById", new { id = NewReligion.Id }, NewReligion);
        }).WithName("CreateReligion").WithOpenApi();

        group.MapPut("/UpdateReligion/{id}", async (IOutputCacheStore cache, [FromServices] ReligionRepository repo, int id, Religion religion, CancellationToken ct) =>
        {
            if (id != religion.Id)
            {
                return Results.BadRequest();
            }

            var updatedReligion = await repo.Update(religion);
            await cache.EvictByTagAsync("Religion", ct);
            return updatedReligion != null ? Results.Ok(updatedReligion) : Results.NotFound();
        })
        .WithName("UpdateReligion");

        group.MapDelete("/DeleteReligion/{id}", async (IOutputCacheStore cache, [FromServices] ReligionRepository repo, int id, CancellationToken ct) =>
        {
            var religion = await repo.GetById(id);
            if (religion == null)
            {
                return Results.NotFound();
            }

            await repo.Delete(id);
            await cache.EvictByTagAsync("Religion", ct);
            return Results.NoContent();
        })
        .WithName("DeleteReligion");
        return group;
    }
}
