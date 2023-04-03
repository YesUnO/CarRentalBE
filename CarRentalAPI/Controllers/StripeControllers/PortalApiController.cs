using Core.Infrastructure.StripePayment;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace CarRentalAPI.Controllers.StripeControllers
{
    [Route("api/stripe/[controller]")]
    [ApiController]
    public class PortalApiController : ControllerBase
    {
        private readonly IStripeService _stripeService;

        public PortalApiController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost]
        public ActionResult Create()
        {
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutService = new SessionService();
            var checkoutSession = checkoutService.Get(Request.Form["session_id"]);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = "http://localhost:4242";

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = returnUrl,
            };
            var service = new Stripe.BillingPortal.SessionService();
            var session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}
