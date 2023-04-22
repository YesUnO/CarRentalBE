using Stripe;

namespace Core.Domain.StripePayments.Interfaces;

public interface IStripeSubscriptionService
{
    string CreateSubscriptionReturnCheckoutSession(string feUrl, string clientMail);
    //string GetSubscriptionId(int applicationUserId);
}
