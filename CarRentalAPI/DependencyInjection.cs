using Core;
using Core.Infrastructure.Authentication;

namespace CarRentalAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddThingsToContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCore(configuration);

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddGoogle(options =>
        //    {
        //        options.ClientId = configuration["ExternalAuthenticationProviders:GoogleClientId"];
        //        options.ClientSecret = configuration["ExternalAuthenticationProviders:GoogleClientSecret"];
        //    })
        //    .AddJwtBearer(options =>
        //    {
        //        options.Authority = "https://localhost:7125";

        //        options.TokenValidationParameters.ValidateAudience = false;

        //        options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
        //    });
        services.AddBffToContainer();

        return services;
     }
}
