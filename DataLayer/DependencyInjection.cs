using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApiResource = Duende.IdentityServer.Models.ApiResource;
using ApiScope = Duende.IdentityServer.Models.ApiScope;

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
                .AddClaimsPrincipalFactory<AdditionalUserClaimsPrincipalFactory>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            var identityResources = new List<Duende.IdentityServer.Models.IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

            var resources = configuration.GetSection("IdentityServer:ApiResources").Get<List<ApiResource>>();
            var scopes = configuration.GetSection("IdentityServer:ApiScopes").Get<List<ApiScope>>();
            var clients = configuration.GetSection("IdentityServer:Clients").Get<List<Duende.IdentityServer.Models.Client>>();

            services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;
            })
                .AddApiAuthorization<IdentityUser, ApplicationDbContext>()
                .AddInMemoryApiScopes(configuration.GetSection("IdentityServer:ApiScopes"))
                .AddInMemoryApiResources(configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryClients(configuration.GetSection("IdentityServer:Clients"))
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
                //})
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
                //})
                .AddInMemoryIdentityResources(identityResources);
                //.AddResourceOwnerValidator < **PasswordAuthentication * *> ();
            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, AdditionalUserClaimsPrincipalFactory>();

            services.AddLocalApiAuthentication();
            return services;
        }
    }
}