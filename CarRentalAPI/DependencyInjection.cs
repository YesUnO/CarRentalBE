using Core;

namespace CarRentalAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddThingsToContainer(this IServiceCollection services)
        {
            services.AddCore();
            return services;
        }
    }
}
