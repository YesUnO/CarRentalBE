//using Core.Domain.User;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Core.ControllerModels.Auth;
//using System.Security.Claims;
//using Core.Exceptions.UserRegistration;
//using Core.Infrastructure.Options;
//using Microsoft.Extensions.Options;
//using Duende.Bff;
//using IdentityModel;

//namespace CarRentalAPI.Controllers;

//[Route("api/[controller]")]
//[BffApi]
//[ApiController]
//public class AuthController : ControllerBase
//{
//    private readonly IUsersService _userService;
//    private readonly ILogger<AuthController> _logger;
//    private readonly BaseApiUrls _baseApiUrls;

//    public AuthController(IUsersService userService,
//                          ILogger<AuthController> logger,
//                          IOptions<BaseApiUrls> baseApiUrls)
//    {
//        _userService = userService;
//        _logger = logger;
//        _baseApiUrls = baseApiUrls.Value;
//    }

    

//    //[HttpGet]
//    //[Route("ConfirmMail")]
//    //[AllowAnonymous]
//    //public async Task<IActionResult> ConfirmMail([FromQuery] ConfirmPasswordModel model)
//    //{
//    //    try
//    //    {
//    //        var user = await _userManager.FindByEmailAsync(model.Email);
//    //        var res = await _userManager.ConfirmEmailAsync(user, model.Token);
//    //        string referrerUrl = Request.Headers["Referer"].ToString();
//    //        return Redirect($"{referrerUrl}/confirmEmail");

//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        _logger.LogError(ex, "Confirming mail failed.");
//    //        return BadRequest();
//    //    }

//    //}


//    //[HttpGet]
//    //[Route("ResendConfirmationEmail")]
//    //[Authorize(Roles = "Customer")]
//    //public async Task<IActionResult> ResendConfirmationEmail()
//    //{
//    //    try
//    //    {
//    //        var loggedinUserMail = HttpContext.User.FindFirstValue(JwtClaimTypes.Email);

//    //        await _userService.ResendConfirmationEmailAsync(loggedinUserMail);
//    //        return Ok(new { Result = "Succesfully send confirm email." });

//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        _logger.LogError(ex, "Confirming mail failed.");
//    //        return BadRequest();
//    //    }

//    //}

//}
