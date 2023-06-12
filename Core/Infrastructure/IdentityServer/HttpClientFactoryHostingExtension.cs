using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.IdentityServer
{
    internal static class HttpClientFactoryHostingExtension
    {
        internal static IServiceCollection AddIdentityServerHttpClientFactory(this IServiceCollection services)
        {
            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("identityServerClient", new ClientCredentialsTokenRequest
                {
                    Address = "https://localhost:5001/connect/token",
                    ClientId = "register",
                });
            });

            services.AddClientAccessTokenHttpClient("identityServerClient", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            });
            return services;
        }
    }
}
