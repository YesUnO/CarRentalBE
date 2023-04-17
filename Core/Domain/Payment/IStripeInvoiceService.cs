using DataLayer.Entities.Orders;

namespace Core.Domain.Payment;

public interface IStripeInvoiceService
{
    StripeInvoice CreateInvoiceForSubscription(string stripePriceId, string subscriptionId, string stripeCustomerId);
}
