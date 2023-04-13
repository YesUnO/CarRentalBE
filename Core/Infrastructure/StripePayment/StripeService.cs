
using Core.Infrastructure.StripePayment.Options;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace Core.Infrastructure.StripePayment;

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;

    public StripeService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
    }

    public string CheckCheckoutSession(string feUrl)
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
            SuccessUrl = feUrl + "?success=true&session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = feUrl + "?canceled=true",
        };
        var service = new SessionService();
        Session session = service.Create(options);
        return session.Url;
    }

    public void Test()
    {
        var yo = "?";
    }
}
