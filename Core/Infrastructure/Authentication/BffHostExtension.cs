using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Authentication
{
    internal static class BffHostExtension
    {
        internal static IServiceCollection AddBffToContainer(this IServiceCollection Services)
        {
            Services.AddBff()
                .AddRemoteApis();

            Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
            .AddCookie("cookie", options =>
            {
                options.Cookie.Name = "__Host-RecipeManagementApp-bff";
                options.Cookie.SameSite = SameSiteMode.Strict;
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "";
                options.ClientId = "";
                options.ClientSecret = "";
                options.ResponseType = "code";
                options.ResponseMode = "query";
                options.UsePkce = true;

                options.GetClaimsFromUserInfoEndpoint = true;
                options.MapInboundClaims = true;
                options.SaveTokens = true;

                //TODO: on publish think through
                options.RequireHttpsMetadata = false;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("cred");
                options.ClaimActions.MapJsonKey("role", "role", "role");
                options.ClaimActions.MapJsonKey("email", "email", "email");
                options.ClaimActions.MapJsonKey("name", "name", "name");

                options.TokenValidationParameters = new()
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });
            return Services;
        }
    }
}
