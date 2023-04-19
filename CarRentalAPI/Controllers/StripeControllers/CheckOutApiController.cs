using CarRentalAPI.Models.Stripe;
using Core.Infrastructure.StripePayment;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalAPI.Controllers.StripeControllers;

[Route("api/stripe/[controller]")]
[ApiController]
public class CheckOutApiController : ControllerBase
{
    private readonly IStripeSubscriptionService _stripeService;
    private readonly ILogger<CheckOutApiController> _logger;

    public CheckOutApiController(IStripeSubscriptionService stripeService, ILogger<CheckOutApiController> logger)
    {
        _stripeService = stripeService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public async Task<IActionResult> Create()
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var domain = Request.Headers["Referer"].ToString();

            var sessionUrl = _stripeService.CheckCheckoutSession(domain, loggedinUserMail);

            //TODO try to implement redirect on release
            //return new SeeOther(sessionUrl);
            return Ok(new CreateCheckoutSessionRasponse { Url = sessionUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "creating checkout session failed.");
            return BadRequest("Checkout session couldnt be created cause of reasons.");
        }
        
    }
}
