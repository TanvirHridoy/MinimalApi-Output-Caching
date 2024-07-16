using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MinimalApi.DIServices;
using MinimalApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(o =>
{
    o.IncludeScopes = true;
    o.IncludeFormattedMessage = true;

});

builder.Services.AppApplicationServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline. 

#region Middlewares

app.MapHealthChecks("/health");
app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
//app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();
#endregion
var endpoints = new EndpointsConfigure();

endpoints.Configure(app);
await app.RunAsync();

