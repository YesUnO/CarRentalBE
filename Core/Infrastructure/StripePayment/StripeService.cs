
using Core.Domain.User;
using Core.Infrastructure.StripePayment.Options;
using DataLayer;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace Core.Infrastructure.StripePayment;

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly UserService _userService;

    public StripeService(IOptions<StripeSettings> stripeSettings, ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager, UserService userService)
    {
        _stripeSettings = stripeSettings.Value;
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _userService = userService;
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
            CheckoutSessionReferenceId= clientMail,
        };

        _applicationDbContext.Add(stripeSubscription);
        var saveToDb = _applicationDbContext.SaveChanges();
        return saveToDb != 0;
    }

    public void Test()
    {
        var yo = "?";
    }
}
