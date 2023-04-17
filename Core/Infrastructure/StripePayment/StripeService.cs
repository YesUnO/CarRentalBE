using Core.Domain.User;
using Core.Infrastructure.StripePayment.Options;
using DataLayer;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Core.Infrastructure.StripePayment;

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IUserService _userService;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IOptions<StripeSettings> stripeSettings, ApplicationDbContext applicationDbContext, IUserService userService, ILogger<StripeService> logger)
    {
        _stripeSettings = stripeSettings.Value;
        _applicationDbContext = applicationDbContext;
        _userService = userService;
        _logger = logger;
    }

    public string CheckCheckoutSession(string feUrl, string clientMail)
    {
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
            ClientReferenceId = clientMail,
            SuccessUrl = feUrl + "?success=true&session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = feUrl + "?canceled=true",
        };
        var service = new SessionService();
        Session session = service.Create(options);
        CreateStripeSubscriptionFromCheckoutSession(clientMail);
        return session.Url;
    }

    private StripeSubscription? GetStripeSubscription(string clientMail)
    {
        var subscription = _applicationDbContext.StripeSubscriptions.FirstOrDefault(x => x.CheckoutSessionReferenceId == clientMail);
        return subscription;
    }

    private bool CreateStripeSubscriptionFromCheckoutSession(string clientMail)
    {
        var loggedinUser = _userService.GetUserByMail(clientMail).Result;
        var stripeSubscription = new StripeSubscription
        {
            ApplicationUser = loggedinUser,
            StripeSubscriptionStatus = StripeSubscriptionStatus.incomplete,
            CheckoutSessionReferenceId = clientMail,
        };

        _applicationDbContext.Add(stripeSubscription);
        var saveToDb = _applicationDbContext.SaveChanges();
        return saveToDb != 0;
    }

    public void Test()
    {
        var yo = "?";
    }

    public void ProcessStripeEvent(Event stripeEvent)
    {
        try
        {
            switch (stripeEvent.Type)
            {
                case Events.CheckoutSessionCompleted:
                    var checkoutSessionEvent = stripeEvent.Data.Object as Session;
                    ProcessCheckoutSessionCompleted(checkoutSessionEvent);
                    break;
                case Events.CustomerSubscriptionDeleted:
                    var subscription = stripeEvent.Data.Object as Subscription;
                    SubscriptionDeleted(subscription);
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        
    }

    private void SubscriptionDeleted(Subscription subscriptionFromevent)
    {
        var service = new SubscriptionService();
        var subscription = service.Get(subscriptionFromevent.Id);
        if (!Enum.TryParse<StripeSubscriptionStatus>(subscription.Status, out var subscriptionStatus))
        {
            throw new Exception("couldnt parse sub status");
        }

        var dbSubscription = _applicationDbContext.StripeSubscriptions
            .FirstOrDefault(s => s.StripeSubscriptionId == subscriptionFromevent.Id);
        if (dbSubscription is null)
        {
            throw new Exception($"subscription {subscriptionFromevent.Id} hasnt been created in db yet... wtf");
        }

        dbSubscription.StripeSubscriptionStatus = subscriptionStatus;
        _applicationDbContext.SaveChanges();
    }

    private void ProcessCheckoutSessionCompleted(Session checkoutSession)
    {
        if (checkoutSession.Mode == "subscription" && checkoutSession.Status == "complete")
        {
            var service = new SubscriptionService();
            var subscription = service.Get(checkoutSession.SubscriptionId);
            var dbSubscription = GetStripeSubscription(checkoutSession.ClientReferenceId);
            if (dbSubscription is null)
            {
                throw new Exception("subscription hasnt been created in db yet... wtf");
            }
            if (!Enum.TryParse<StripeSubscriptionStatus>(subscription.Status, out var subscriptionStatus))
            {
                throw new Exception("couldnt parse sub status");
            }
            if (checkoutSession.SubscriptionId is null)
            {
                throw new Exception("couldnt parse subscription  id from webhook");
            }
            dbSubscription.StripeSubscriptionStatus = subscriptionStatus;
            dbSubscription.StripeSubscriptionId = checkoutSession.SubscriptionId;
            _applicationDbContext.SaveChanges();
        }
    }
}
