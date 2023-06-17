using Core.ControllerModels.Stripe;
using Core.Domain.Payment.Options;
using Core.Domain.StripePayments.Interfaces;
using Duende.Bff;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using System.Security.Claims;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[BffApi]
[ApiController]
public class StripeController : ControllerBase
{
    private readonly IStripeSubscriptionService _subscriptionService;
    private readonly IStripeWebhookService _webhookService;
    private readonly ILogger<StripeController> _logger;
    private readonly StripeSettings _stripeSettings;

    public StripeController(IStripeSubscriptionService stripeService,
                            ILogger<StripeController> logger,
                            IStripeWebhookService webhookService,
                            IOptions<StripeSettings> stripeOptions)
    {
        _subscriptionService = stripeService;
        _logger = logger;
        _webhookService = webhookService;
        _stripeSettings = stripeOptions.Value;
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateSubscription()
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(JwtClaimTypes.Email);
            var domain = Request.Headers["Referer"].ToString();

            var sessionUrl = _subscriptionService.CreateSubscriptionReturnCheckoutSession(domain, loggedinUserMail);

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


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        const string endpointSecret = "whsec_12096727fe2e79f76f75b27162f537212b0c4e4259e8891117ad8d47cb5c4823";
        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];
            stripeEvent = EventUtility.ConstructEvent(json,
                    signatureHeader, endpointSecret);

            //TODO: use secret from appsettings on release
            //stripeEvent = EventUtility.ConstructEvent(json,
            //        signatureHeader, _stripeSettings.EndpointSecret);

            _webhookService.ProcessStripeEvent(stripeEvent);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Parsing stripe webhooks failed", ex.Message);
            return BadRequest();
        }
    }
}
