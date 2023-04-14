using CarRentalAPI.Helpers;
using CarRentalAPI.Models.Stripe;
using Core.Infrastructure.StripePayment;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers.StripeControllers;

[Route("api/stripe/[controller]")]
[ApiController]
public class CheckOutApiController : ControllerBase
{
    private readonly IStripeService _stripeService;

    public CheckOutApiController(IStripeService stripeService)
    {
        _stripeService = stripeService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var domain = Request.Headers["Referer"].ToString();

        var sessionUrl = _stripeService.CheckCheckoutSession(domain);

        return Ok(new CreateCheckoutSessionRasponse { Url = sessionUrl });
        //return new SeeOther(sessionUrl);
    }
}
