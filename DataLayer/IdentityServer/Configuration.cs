using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace DataLayer.IdentityServer
{
    internal class Configuration
    {
        internal List<ApiScope> GetApiScopes()
        {
            return new List<ApiScope> { 
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName) 
            };
        }

        internal List<Client> GetClients()
        {
            return new List<Client> { 
                new Client {
                    ClientId = "local-dev",
                    AllowedGrantTypes ={ GrantType.ResourceOwnerPassword },
                    ClientSecrets ={new Secret("yo".Sha256())},
                    RequireClientSecret= false,
                    AllowedScopes=  { IdentityServerConstants.LocalApi.ScopeName }
                } 
            };
        }
    }
}
