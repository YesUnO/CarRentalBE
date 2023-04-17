using Core.Infrastructure.StripePayment;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CarRentalAPI.Controllers.StripeControllers;

[Route("api/stripe/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly IStripeSubscriptionService _stripeService;

    public WebhookController(IStripeSubscriptionService stripeService)
    {
        _stripeService = stripeService;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        // Replace this endpoint secret with your endpoint's unique secret
        // If you are testing with the CLI, find the secret by running 'stripe listen'
        // If you are using an endpoint defined with the API or dashboard, look in your webhook settings
        // at https://dashboard.stripe.com/webhooks
        const string endpointSecret = "whsec_12096727fe2e79f76f75b27162f537212b0c4e4259e8891117ad8d47cb5c4823";
        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];
            stripeEvent = EventUtility.ConstructEvent(json,
                    signatureHeader, endpointSecret);

            _stripeService.ProcessStripeEvent(stripeEvent);
            
            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine("Error: {0}", e.Message);
            return BadRequest();
        }
    }
}
