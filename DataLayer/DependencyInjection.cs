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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ViewOwnOrdersPolicy", policy =>
                policy.Requirements.Add(new ViewOwnOrdersRequirement()));
        });
        return services;
    }
}