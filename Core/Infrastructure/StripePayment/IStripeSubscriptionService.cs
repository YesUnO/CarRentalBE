
using Stripe;

namespace Core.Infrastructure.StripePayment;

public interface IStripeSubscriptionService
{
    string CheckCheckoutSession(string feUrl, string clientMail);
    void ProcessStripeEvent(Event stripeEvent);
    //string GetSubscriptionId(int applicationUserId);
}
