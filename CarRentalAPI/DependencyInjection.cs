using Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddThingsToContainer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCore(configuration);


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
}
