using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
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

            var identityResources = new List<Duende.IdentityServer.Models.IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

            services.AddIdentityServer()
                .AddApiAuthorization<IdentityUser, ApplicationDbContext>()
                .AddInMemoryApiResources(configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryClients(configuration.GetSection("IdentityServer:Clients"));
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
                //})
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
                //})
                //.AddInMemoryIdentityResources(identityResources);

            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}