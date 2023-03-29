using Microsoft.Extensions.DependencyInjection;
using DataLayer;
using Microsoft.Extensions.Configuration;
using Core.Domain.Cars;
using Core.Domain.User;
using Core.Domain.Payment;
using Core.Infrastructure.Files;
using Core.Infrastructure.Options;
using Core.Domain.Orders;

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
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IOrderService, OrderService>();

            services.AddTransient<IFileService, FileService>();
            services.Configure<FileSettings>(configuration.GetSection("FileSettings"));
            return services;
        }
    }
}