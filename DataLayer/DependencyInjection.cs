using DataLayer.IdentityServer.AuthorizationPolicies.OwnOrdersRequirement;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

        services.AddTransient<IAuthorizationHandler, ViewOwnOrdersHandler>();

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(o =>
           o.TokenLifespan = TimeSpan.FromMinutes(30));

        var identityResources = new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        var client = configuration.GetSection("IdentityServer:Clients").Get<List<Client>>();

        services.AddIdentityServer(options =>
        {
            options.EmitStaticAudienceClaim = true;
        })
            .AddApiAuthorization<IdentityUser, ApplicationDbContext>()
            .AddInMemoryApiScopes(configuration.GetSection("IdentityServer:ApiScopes"))
            .AddInMemoryClients(configuration.GetSection("IdentityServer:Clients"))
            .AddInMemoryIdentityResources(identityResources);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ViewOwnOrdersPolicy", policy =>
                policy.Requirements.Add(new ViewOwnOrdersRequirement()));
        });
        return services;
    }
}