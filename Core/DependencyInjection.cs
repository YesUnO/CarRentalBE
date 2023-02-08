using Microsoft.Extensions.DependencyInjection;
using DataLayer;
using Core.Services.Interfaces;
using Core.Services;

namespace Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddDataLayer();
            return services;
        }
    }
} 