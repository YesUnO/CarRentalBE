using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CarRentalContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnectionString")));
            return services;
        }
    }
}