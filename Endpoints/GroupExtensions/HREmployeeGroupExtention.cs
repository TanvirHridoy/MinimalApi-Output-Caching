using Microsoft.AspNetCore.Mvc;
using MinimalApi.CustomMiddleware;
using MinimalApi.DTO;
using MinimalApi.Extensions;
using MinimalApi.Interfaces;
using MinimalApi.Repository;

namespace MinimalApi.Endpoints.GroupExtensions;

public static class HREmployeeGroupExtention
{
    public static RouteGroupBuilder MapHREmployeeGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/GetAll", async ([FromServices] CustomApiMetrics customMetrics, [FromServices] EmployeeRepository repo, [FromServices] ITokenService tokenService) =>
        {
            using var _ = customMetrics.MeasureRequestDuration();
            try
            {
                var employees = await repo.GetAllEmployees();

                return Results.Ok(employees);
            }
            finally
            {
                customMetrics.InceaseEmployeeRequestCount();
            }


        }).WithName("GetAllEmployees").RequireRateLimiting("token-based").WithMetadata(new TokenBasedRateLimitingAttribute());
        //group.MapGet("/GetAllEmployees", async (HttpContext httpContext, [FromServices] CustomApiMetrics customMetrics, [FromServices] EmployeeRepository repo, [FromServices] ITokenService tokenService) =>
        //{
        //    using var _ = customMetrics.MeasureRequestDuration();
        //    try
        //    {
        //        // Get the username from the authenticated user
        //        var username = httpContext.User.Identity?.Name;
        //        if (string.IsNullOrEmpty(username))
        //        {
        //            return Results.Unauthorized();
        //        }

        //        // Attempt to consume a token
        //        bool tokenConsumed = await tokenService.ConsumeTokenAsync(username);
        //        if (!tokenConsumed)
        //        {
        //            return Results.StatusCode(StatusCodes.Status429TooManyRequests);
        //        }

        //        // If token was successfully consumed, proceed with the original logic
        //        var employees = await repo.GetAllEmployees();
        //        return Results.Ok(employees);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        // You might want to use a logging framework here
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //        return Results.StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //    finally
        //    {
        //        customMetrics.InceaseEmployeeRequestCount();
        //    }
        //}).WithName("GetAllEmployees").RequireAuthorization();

        group.MapGet("/GetById/{id}", async ([FromServices] EmployeeRepository repo, int id) =>
        {
            var employee = await repo.GetEmployeeById(id);
            return employee != null ? Results.Ok(employee) : Results.NotFound();
        }).WithName("GetEmployeeById");

        group.MapPost("/Create", async ([FromServices] EmployeeRepository repo, Employee employee) =>
        {

            var newEmployee = await repo.AddEmployee(employee);
            return Results.CreatedAtRoute("GetById", new { id = newEmployee.EmployeeId }, newEmployee);
        })
                    .WithName("CreateEmployee")
                    .WithOpenApi();

        group.MapPut("/Update/{id}", async ([FromServices] EmployeeRepository repo, int id, Employee employee) =>
        {
            if (id != employee.EmployeeId)
            {
                return Results.BadRequest();
            }

            var updatedEmployee = await repo.UpdateEmployee(employee);
            return updatedEmployee != null ? Results.Ok(updatedEmployee) : Results.NotFound();
        })
        .WithName("UpdateEmployee");

        group.MapDelete("/{id}", async ([FromServices] EmployeeRepository repo, int id) =>
        {
            var employee = await repo.GetEmployeeById(id);
            if (employee == null)
            {
                return Results.NotFound();
            }

            await repo.DeleteEmployee(id);
            return Results.NoContent();
        })
        .WithName("Delete");

        return group;
    }
}
