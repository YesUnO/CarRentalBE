using Core.Infrastructure.StripePayment;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;

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
    public ActionResult Create()
    {
        var domain = "http://localhost:4242";

        var priceOptions = new PriceListOptions
        {
            LookupKeys = new List<string> {
                    Request.Form["lookup_key"]
                }
        };
        var priceService = new PriceService();
        StripeList<Price> prices = priceService.List(priceOptions);

        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = prices.Data[0].Id,
                    Quantity = 1,
                  },
                },
            Mode = "subscription",
            SuccessUrl = domain + "?success=true&session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = domain + "?canceled=true",
        };
        var service = new SessionService();
        Session session = service.Create(options);

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }
}
