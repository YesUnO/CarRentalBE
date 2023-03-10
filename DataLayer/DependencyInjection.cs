using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var clientSection = configuration.GetSection("IdentityServer");

            var yo = clientSection.Get<Client[]>();

            services.AddIdentityServer()
                .AddApiAuthorization<IdentityUser, ApplicationDbContext>()
                .AddInMemoryClients(configuration.GetSection("IdentityServer:Clients"));

            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}