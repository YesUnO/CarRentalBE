using Core.Domain.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Duende.IdentityServer;
using Core.ControllerModels.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Core.Exceptions.UserRegistration;
using Core.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.Services;
using Newtonsoft.Json;
using IdentityModel.Client;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;
    private readonly BaseApiUrls _baseApiUrls;
    private readonly IIdentityServerInteractionService _identityServerInteractionService;

    public AuthController(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          IUserService userService,
                          ILogger<AuthController> logger,
                          IOptions<BaseApiUrls> baseApiUrls,
                          IIdentityServerInteractionService identityServerInteractionService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _logger = logger;
        _baseApiUrls = baseApiUrls.Value;
        _identityServerInteractionService = identityServerInteractionService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        try
        {
            var user = new IdentityUser(model.UserName);
            await _userService.RegisterCustomer(user, model.Email, model.Password);
            await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            return Ok(new { Result = "Succesfully registered a new customer." });
        }
        catch (UserRegistrationException ex)
        {
            var errors = new RegisterErrorResponseModel();
            foreach (var error in ex.Errors)
            {
                switch (error.Field)
                {
                    case "email":
                        if (errors.Email is null)
                        {
                            errors.Email = new List<string>();
                        }
                        ((List<string>)errors.Email).Add(error.Description);
                        break;
                    case "password":
                        if (errors.Password is null)
                        {
                            errors.Password = new List<string>();
                        }
                        ((List<string>)errors.Password).Add(error.Description);
                        break;
                    case "username":
                        if (errors.Username is null)
                        {
                            errors.Username = new List<string>();
                        }
                        ((List<string>)errors.Username).Add(error.Description);
                        break;
                    default:
                        break;
                }
            }
            return BadRequest(new { errors } );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registering new user failed.");
            return BadRequest();
        }

    }

    //[HttpPost]
    //[Route("ExternalLogin")]
    //[AllowAnonymous]
    //public async Task<IActionResult> ExternalLogin(ExternalLoginRequestModel model)
    //{
    //    try
    //    {
    //        var identity = await _userService.HandleExternalLoginAsync(model.Credentials);
    //        var user = new IdentityServerUser(identity.Id)
    //        {
    //            DisplayName = identity.UserName,
    //        };

    //        await HttpContext.SignInAsync(user);
    //        var response = await HttpContext.GetTokenAsync(IdentityServerConstants.TokenTypes.AccessToken);
    //        return Ok(response);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "External login failed.");
    //        return BadRequest();
    //    }
    //}

    [HttpGet]
    [Route("ExternalLogin")]
    public IActionResult GoogleLogin()
    {
        string referrerUrl = Request.Headers["Referer"].ToString();
        var redirectUrl = $"{_baseApiUrls.HttpsUrl}/api/auth/ExternalLoginCallback?redirectUrl={referrerUrl}";
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        properties.AllowRefresh = true;
        return Challenge(properties,"Google");
    }

    [HttpGet]
    [Route("ExternalLoginCallback")]
    public async Task<IActionResult> GoogleLoginCallback(string redirectUrl)
    {
        ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
        
        var identity = await _userService.HandleExternalLoginAsync(info);
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        var client = new HttpClient();
        //var disco = await client.GetDiscoveryDocumentAsync(_baseApiUrls.HttpsUrl);
        var credentialsTokenRequest = new ClientCredentialsTokenRequest
        {
            Address = $"{_baseApiUrls.HttpsUrl}/connect/token",

            ClientId = "local-dev",
        };
        var tokenResponse = await client.RequestClientCredentialsTokenAsync(credentialsTokenRequest);
        return Redirect(redirectUrl);
    }

    [HttpGet]
    [Route("ConfirmMail")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmMail([FromQuery] ConfirmPasswordModel model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var res = await _userManager.ConfirmEmailAsync(user, model.Token);
            string referrerUrl = Request.Headers["Referer"].ToString();
            return Redirect($"{referrerUrl}/confirmEmail");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Confirming mail failed.");
            return BadRequest();
        }

    }


    [HttpGet]
    [Route("ResendConfirmationEmail")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public async Task<IActionResult> ResendConfirmationEmail()
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(loggedinUserMail);

            await _userService.ResendConfirmationEmailAsync(user);
            return Ok(new { Result = "Succesfully send confirm email." });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Confirming mail failed.");
            return BadRequest();
        }

    }


    //public async Task<IActionResult> Login(Login model)
    //{
    //    var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);

    //    if (result.Succeeded)
    //    {
    //        var user = await _signInManager.UserManager.FindByNameAsync(model.Username);
    //        var token = GenerateToken(user);
    //        return Ok(new { token });
    //    }
    //    return Unauthorized();
    //}
    //private string GenerateToken(IdentityUser user)
    //{
    //    var claims = new[]
    //{
    //    new Claim(ClaimTypes.NameIdentifier, user.Id),
    //    new Claim(ClaimTypes.Name, user.UserName),
    //};

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"));
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(
    //        issuer: "your-website-url",
    //        audience: "your-website-url",
    //        claims: claims,
    //        expires: DateTime.Now.AddMinutes(30),
    //        signingCredentials: creds);

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}
}
