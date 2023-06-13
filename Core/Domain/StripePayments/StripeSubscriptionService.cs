using Core.Domain.Payment.Options;
using Core.Domain.StripePayments.Interfaces;
using Core.Domain.User;
using DataLayer;
using DataLayer.Entities.User;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace Core.Domain.StripePayments;

public class StripeSubscriptionService : IStripeSubscriptionService
{
    private readonly StripeSettings _stripeSettings;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IUsersService _userService;
    private readonly ILogger<StripeSubscriptionService> _logger;

    public StripeSubscriptionService(IOptions<StripeSettings> stripeSettings,
                                     ApplicationDbContext applicationDbContext,
                                     IUsersService userService,
                                     ILogger<StripeSubscriptionService> logger)
    {
        _stripeSettings = stripeSettings.Value;
        _applicationDbContext = applicationDbContext;
        _userService = userService;
        _logger = logger;
    }

    public string CreateSubscriptionReturnCheckoutSession(string feUrl, string clientMail)
    {
        var clientReferenceId = Guid.NewGuid().ToString();
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = _stripeSettings.CarRentalPriceId,
                    Quantity = 1,
                  },

                },
            Mode = "subscription",
            ClientReferenceId = clientReferenceId,
            SuccessUrl = feUrl + "?success=true&session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = feUrl + "?canceled=true",
        };
        var service = new SessionService();
        Session session = service.Create(options);
        CreateStripeSubscriptionFromCheckoutSession(clientReferenceId, clientMail);
        return session.Url;
    }

    private bool CreateStripeSubscriptionFromCheckoutSession(string clientReferenceId, string clientMail)
    {
        var loggedinUser = _userService.GetUserByMailAsync(clientMail).Result;
        var stripeSubscription = new StripeSubscription
        {
            ApplicationUser = loggedinUser,
            StripeSubscriptionStatus = StripeSubscriptionStatus.incomplete,
            CheckoutSessionReferenceId = clientReferenceId,
        };

        _applicationDbContext.Add(stripeSubscription);
        var saveToDb = _applicationDbContext.SaveChanges();
        return saveToDb != 0;
    }

}
