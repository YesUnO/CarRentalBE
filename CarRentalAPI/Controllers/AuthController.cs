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

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          IUserService userService,
                          ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        try
        {
            var user = new IdentityUser(model.UserName);
            var action = Url.Action("ConfirmMail");
            var url = $"{Request.Scheme}://{Request.Host}{action}";
            await _userService.RegisterCustomer(user, model.Email, model.Password, url);
            await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            var response = await HttpContext.GetTokenAsync(IdentityServerConstants.TokenTypes.AccessToken);
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
            return BadRequest(new { errors = errors } );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registering new user failed.");
            return BadRequest();
        }

    }

    [HttpPost]
    [Route("ExternalLogin")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLogin()
    {
        var yo = _signInManager.GetExternalLoginInfoAsync();
        return Ok();
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
            var feUrl = Environment.GetEnvironmentVariable("FE");
            return Redirect($"{feUrl}/confirmEmail");

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

            var action = Url.Action("ConfirmMail");
            var url = $"{Request.Scheme}://{Request.Host}{action}";
            await _userService.ResendConfirmationEmailAsync(user, url);
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
