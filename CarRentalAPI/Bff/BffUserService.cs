using DataLayer;
using DataLayer.Entities.User;
using Duende.Bff;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net.Http;
using System.Security.Claims;

namespace CarRentalAPI.Bff
{
    public class BffUserService : DefaultUserService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public BffUserService(IOptions<BffOptions> options,
                              ILoggerFactory loggerFactory,
                              ApplicationDbContext applicationDbContext) : base(options, loggerFactory)
        {
            _applicationDbContext = applicationDbContext;
        }

        public override Task ProcessRequestAsync(HttpContext context)
        {

            string authnMethodsReferences = context.User.FindFirstValue("http://schemas.microsoft.com/claims/authnmethodsreferences");
            Claim emailClaim = context.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email);
            var nameClaim = context.User.Identity.Name;

            if (authnMethodsReferences != null && authnMethodsReferences == "external" && emailClaim != null)
            {
                var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x=>x.Email == emailClaim.Value);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email= emailClaim.Value,
                        Username = nameClaim,
                    };
                    _applicationDbContext.ApplicationUsers.Add(user);
                    _applicationDbContext.SaveChanges();
                }
            }

            return base.ProcessRequestAsync(context);
        }
    }
}
