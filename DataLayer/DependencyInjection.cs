using Duende.IdentityServer.Models;
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

            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, AdditionalUserClaimsPrincipalFactory>();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddClaimsPrincipalFactory<AdditionalUserClaimsPrincipalFactory>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            var identityResources = new List<Duende.IdentityServer.Models.IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

            services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;
            })
                .AddApiAuthorization<IdentityUser, ApplicationDbContext>()
                .AddInMemoryApiScopes(configuration.GetSection("IdentityServer:ApiScopes"))
                .AddInMemoryApiResources(configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryClients(configuration.GetSection("IdentityServer:Clients"))
                .AddInMemoryIdentityResources(identityResources);
                //.AddResourceOwnerValidator < **PasswordAuthentication * *> ();

            services.AddLocalApiAuthentication();
            return services;
        }
    }
}