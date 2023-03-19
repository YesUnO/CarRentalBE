using Core;
using Microsoft.AspNetCore.Authentication;

namespace CarRentalAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddThingsToContainer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCore(configuration);


            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
         }
    }
}
