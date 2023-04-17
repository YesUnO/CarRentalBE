using CarRentalAPI.Models.Stripe;
using Core.Infrastructure.StripePayment;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalAPI.Controllers.StripeControllers;

[Route("api/stripe/[controller]")]
[ApiController]
public class CheckOutApiController : ControllerBase
{
    private readonly IStripeSubscriptionService _stripeService;

    public CheckOutApiController(IStripeSubscriptionService stripeService)
    {
        _stripeService = stripeService;
    }

    [HttpGet]
    //TODO implement authorization
    public async Task<IActionResult> Create()
    {
        var loggedinUserMail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
        var domain = Request.Headers["Referer"].ToString();

        var sessionUrl = _stripeService.CheckCheckoutSession(domain, "vilem@gmail.com");

        return Ok(new CreateCheckoutSessionRasponse { Url = sessionUrl });
        //return new SeeOther(sessionUrl);
    }
}
