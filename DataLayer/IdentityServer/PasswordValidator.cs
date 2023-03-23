using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DataLayer.IdentityServer
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<PasswordValidator> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public PasswordValidator(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<PasswordValidator> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            _logger.LogInformation("yo from the yo");
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
                            claims.Add(new Claim(JwtClaimTypes.Role, role));

                        }
                    }
                    Dictionary<string, object> yo = claims.ToDictionary(x => x.Type, y => (object)y.Value);
                    // context set to success
                    context.Result = new GrantValidationResult(
                        subject: user.Id.ToString(),
                        authenticationMethod: "custom",
                        claims: claims
                    );
                    _logger.LogInformation("yo from the ho",context);

                    return;
                }
            }

        }
    }
}
