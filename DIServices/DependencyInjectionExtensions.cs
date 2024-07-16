using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.CustomPolicy;
using MinimalApi.DTO;
using MinimalApi.Extensions;
using MinimalApi.Interfaces;
using MinimalApi.Repository;
using MinimalApi.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Text;

namespace MinimalApi.DIServices;

public static class DependencyInjectionExtensions
{

    public static IServiceCollection AppApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAppMetricsServices();

        services.AddDbContext<EmployeeDbContext>(o => o.UseSqlServer(config.GetConnectionString("EmployeeDb")));
        services.AddIdentity<ApplicationUser, ApplicationUserRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
                .AddEntityFrameworkStores<EmployeeDbContext>()
                .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "JwtBearer";
            options.DefaultChallengeScheme = "JwtBearer";
        }).AddJwtBearer("JwtBearer", jwtoptions =>
        {
            jwtoptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:EncryptionKey"]))
            };
        });
        services.AddAuthorization();

        services.AddScoped<JwtTokenService>();


        #region rate Limiting
        services.AddRateLimiter(options =>
        {
            options.AddPolicy<string, TokenBasedRateLimiter>("token-based");
        });

        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<ILogger<TokenBasedRateLimiter>, Logger<TokenBasedRateLimiter>>();
        #endregion

        services.AddScoped<EmployeeRepository>();
        services.AddScoped<ReligionRepository>();

        #region Commented
        //services.AddOutputCache(opt =>
        //{
        //    opt.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(10)));
        //    opt.AddPolicy("ReligionPolicy", builder =>
        //        builder.Tag("Religion")
        //               .Expire(TimeSpan.FromMinutes(10))
        //                .VaryByValue(ctx =>ctx.User.Identity.IsAuthenticated?"Authorizeduser":"Anonymous" )
        //               );
        //});
        //services.AddOutputCache(opt =>
        //{
        //    opt.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(10)));
        //    opt.AddPolicy("ReligionPolicy", builder =>
        //        builder.Tag("Religion")
        //               .Expire(TimeSpan.FromMinutes(10))
        //               .VaryByValue(ctx =>
        //               {
        //                   var isAuthenticated = ctx.User?.Identity?.IsAuthenticated ?? false;
        //                   return new KeyValuePair<string, string>(
        //                       "UserType",
        //                       isAuthenticated ? "AuthorizedUser" : "AnonymousUser"
        //                   );
        //               })
        //               .SetVaryByHeader("Authorization")
        //       );
        //});
        //services.AddOutputCache(opt =>
        //{
        //    opt.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(10)));
        //    opt.AddPolicy("ReligionPolicy", builder =>
        //        builder.Tag("Religion")
        //               .Expire(TimeSpan.FromMinutes(10))
        //               .SetVaryByHeader("Authorization")
        //    );
        //});
        #endregion

        services.AddOutputCache(opt =>
        {
            opt.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(10)));// Base policy
            
            opt.AddPolicy("ReligionPolicy", builder =>
                builder.AddPolicy<AuthenticatedCachePolicy>()
                       .Tag("Religion")
                       .Expire(TimeSpan.FromMinutes(30)));
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static IServiceCollection AddAppMetricsServices(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithMetrics(o =>
        {
            o.AddRuntimeInstrumentation()
            .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http",
                "MinimalApi"//custom
                );
        }).WithTracing(t =>
        {
            t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();
        });



        services.Configure<OpenTelemetryLoggerOptions>(log => log.AddOtlpExporter());
        services.ConfigureOpenTelemetryMeterProvider(m => m.AddOtlpExporter());
        services.ConfigureOpenTelemetryTracerProvider(tracer => tracer.AddOtlpExporter());

        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        services.ConfigureHttpClientDefaults(h =>
        {
            h.AddStandardResilienceHandler();
        });

        services.AddMetrics();
        services.AddSingleton<CustomApiMetrics>();
        return services;
    }
}
