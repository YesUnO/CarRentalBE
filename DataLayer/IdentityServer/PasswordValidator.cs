using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static IdentityModel.OidcConstants;

namespace DataLayer.IdentityServer
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public PasswordValidator(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var result = await _signInManager.PasswordSignInAsync(context.UserName, context.Password, isPersistent: true, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(context.UserName);
                
                if (user != null)
                {
                    var claims = await _userManager.GetClaimsAsync(user);
                    var rolez = _userManager.GetRolesAsync(user).Result;

                    if (rolez.Any())
                    {
                        foreach (var role in rolez)
                        {
                            claims.Append(new Claim(JwtClaimTypes.Role, role));

                        }
                    }
                    // context set to success
                    context.Result = new GrantValidationResult(
                        subject: user.Id.ToString(),
                        authenticationMethod: AuthenticationMethods.Password,
                        claims: claims
                    );
                    return;
                }
            }

        }
    }
}
