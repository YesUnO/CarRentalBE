using Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CarRentalAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddThingsToContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCore(configuration);

        services.AddHttpContextAccessor();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:7125";

                options.TokenValidationParameters.ValidateAudience = false;

                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
            });

        return services;
     }
}
