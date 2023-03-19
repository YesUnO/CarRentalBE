using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DataLayer
{
    public class AdditionalUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
    {

        public AdditionalUserClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);

            var rolez = await UserManager.GetRolesAsync(user);
            if (rolez.Any())
            {
                foreach (var role in rolez)
                {
                    claimsIdentity.AddClaim(new Claim(JwtClaimTypes.Role, role));
                }
            }
            return claimsIdentity;
        }
    }
}
