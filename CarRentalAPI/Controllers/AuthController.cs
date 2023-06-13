using Core.Domain.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.ControllerModels.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Core.Exceptions.UserRegistration;
using Core.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Duende.Bff;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[BffApi]
public class AuthController : ControllerBase
{
    private readonly IUsersService _userService;
    private readonly ILogger<AuthController> _logger;
    private readonly BaseApiUrls _baseApiUrls;

    public AuthController(IUsersService userService,
                          ILogger<AuthController> logger,
                          IOptions<BaseApiUrls> baseApiUrls)
    {
        _userService = userService;
        _logger = logger;
        _baseApiUrls = baseApiUrls.Value;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        try
        {
            var user = new IdentityUser(model.UserName);
            await _userService.RegisterCustomer(model.Email, model.Password, model.UserName);
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

    //[HttpGet]
    //[Route("ConfirmMail")]
    //[AllowAnonymous]
    //public async Task<IActionResult> ConfirmMail([FromQuery] ConfirmPasswordModel model)
    //{
    //    try
    //    {
    //        var user = await _userManager.FindByEmailAsync(model.Email);
    //        var res = await _userManager.ConfirmEmailAsync(user, model.Token);
    //        string referrerUrl = Request.Headers["Referer"].ToString();
    //        return Redirect($"{referrerUrl}/confirmEmail");

    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Confirming mail failed.");
    //        return BadRequest();
    //    }

    //}


    [HttpGet]
    [Route("ResendConfirmationEmail")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ResendConfirmationEmail()
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            await _userService.ResendConfirmationEmailAsync(loggedinUserMail);
            return Ok(new { Result = "Succesfully send confirm email." });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Confirming mail failed.");
            return BadRequest();
        }

    }
}
