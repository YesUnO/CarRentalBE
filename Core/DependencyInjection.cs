using Microsoft.Extensions.DependencyInjection;
using DataLayer;
using Core.Services.Interfaces;
using Core.Services;
using Microsoft.Extensions.Configuration;

namespace Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddDataLayer(configuration);
            return services;
        }
    }
}