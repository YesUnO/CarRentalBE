using DataLayer.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
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

            //services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, AdditionalUserClaimsPrincipalFactory>();
            //services.AddScoped<IResourceOwnerPasswordValidator, PasswordValidator>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                //.AddClaimsPrincipalFactory<AdditionalUserClaimsPrincipalFactory>()
                //.AddDefaultTokenProviders()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            var identityResources = new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

            services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;
            })
                .AddApiAuthorization<IdentityUser, ApplicationDbContext>()
                .AddInMemoryApiScopes(configuration.GetSection("IdentityServer:ApiScopes"))
                //.AddInMemoryApiResources(configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryClients(configuration.GetSection("IdentityServer:Clients"))
                //.AddResourceOwnerValidator<PasswordValidator>()
                .AddInMemoryIdentityResources(identityResources);

            services.AddLocalApiAuthentication();
            return services;
        }
    }
}