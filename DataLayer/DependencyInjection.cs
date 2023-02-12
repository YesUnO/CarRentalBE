using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace DataLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services)
        {
            services.AddDbContext<CarRentalContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("BloggingContext")));
            return services;
        }
    }
}