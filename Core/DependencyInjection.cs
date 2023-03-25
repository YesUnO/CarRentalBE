using Microsoft.Extensions.DependencyInjection;
using DataLayer;
using Microsoft.Extensions.Configuration;
using Core.User;
using Core.Files;
using Core.Cars;
using Core.Payment;

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
            services.AddTransient<IPaymentService, PaymentService>();

            return services;
        }
    }
}