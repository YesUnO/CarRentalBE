using Core;

namespace CarRentalAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddThingsToContainer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCore(configuration);
            return services;
        }
    }
}
