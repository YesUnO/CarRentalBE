using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;

namespace Core.Infrastructure.Authentication
{
    internal static class BffHostExtension
    {
        internal static IServiceCollection AddBffToContainer(this IServiceCollection Services, IConfiguration configuration)
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
                options.Cookie.Name = "__CarRentalApp-bff";
                options.Cookie.SameSite = SameSiteMode.Strict;
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = configuration.GetValue<string>("IdentityServer:Authority");
                options.ClientId = configuration.GetValue<string>("IdentityServer:ClientId");
                options.ClientSecret = configuration.GetValue<string>("IdentityServer:ClientSecret");
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
                options.ClaimActions.MapJsonKey("email_verified", "email_verified", "email_verified");
                options.ClaimActions.MapJsonKey("role", "role", "role");

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
