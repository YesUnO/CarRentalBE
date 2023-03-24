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
            services.AddDataLayer(configuration);

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICardService, CardService>();
            services.AddTransient<ICarService, CarService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IPaymentService, PaymentService>();

            return services;
        }
    }
}