using CarRentalAPI.Helpers;
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

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var domain = Request.Headers["Referer"].ToString();

        var sessionUrl = _stripeService.CheckCheckoutSession(domain);

        return new SeeOther(sessionUrl);
    }
}
