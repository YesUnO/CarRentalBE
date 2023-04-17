
using Stripe;

namespace Core.Infrastructure.StripePayment;

public interface IStripeService
{
    void Test();
    string CheckCheckoutSession(string feUrl, string clientMail);
    void ProcessStripeEvent(Event stripeEvent);
}
