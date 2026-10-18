using Api.Modules.Errors;
using CinemaApi.Modules.Errors;
using Microsoft.Extensions.DependencyInjection;
namespace CinemaApi.Modules;

public static class SetupModule
{
    public static void SetupServices(this IServiceCollection services)
    {
        services.AddControllers(options => 
            options.Filters.Add<ValidationExceptionFilter>());
            
        services.AddCorsConfig();
    }

    private static void AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));
    }
}