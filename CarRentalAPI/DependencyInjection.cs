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
                    // base-address of your identityserver
                    options.Authority = "https://localhost:7125";

                    // audience is optional, make sure you read the following paragraphs
                    // to understand your options
                    options.TokenValidationParameters.ValidateAudience = false;

                    // it's recommended to check the type header to avoid "JWT confusion" attacks
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                });

            return services;
         }
    }
}
