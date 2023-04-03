using Core.Infrastructure.StripePayment;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CarRentalAPI.Controllers.StripeControllers;

[Route("api/stripe/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly IStripeService _stripeService;

    public WebhookController(IStripeService stripeService)
    {
        _stripeService = stripeService;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        _stripeService.Test();
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        // Replace this endpoint secret with your endpoint's unique secret
        // If you are testing with the CLI, find the secret by running 'stripe listen'
        // If you are using an endpoint defined with the API or dashboard, look in your webhook settings
        // at https://dashboard.stripe.com/webhooks
        const string endpointSecret = "whsec_12345";
        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];
            stripeEvent = EventUtility.ConstructEvent(json,
                    signatureHeader, endpointSecret);
            if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
            {
                var subscription = stripeEvent.Data.Object as Subscription;
                Console.WriteLine("A subscription was canceled.", subscription.Id);
                // Then define and call a method to handle the successful payment intent.
                // handleSubscriptionCanceled(subscription);
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
            {
                var subscription = stripeEvent.Data.Object as Subscription;
                Console.WriteLine("A subscription was updated.", subscription.Id);
                // Then define and call a method to handle the successful payment intent.
                // handleSubscriptionUpdated(subscription);
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
            {
                var subscription = stripeEvent.Data.Object as Subscription;
                Console.WriteLine("A subscription was created.", subscription.Id);
                // Then define and call a method to handle the successful payment intent.
                // handleSubscriptionUpdated(subscription);
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionTrialWillEnd)
            {
                var subscription = stripeEvent.Data.Object as Subscription;
                Console.WriteLine("A subscription trial will end", subscription.Id);
                // Then define and call a method to handle the successful payment intent.
                // handleSubscriptionUpdated(subscription);
            }
            else
            {
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }
            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine("Error: {0}", e.Message);
            return BadRequest();
        }
    }
}
