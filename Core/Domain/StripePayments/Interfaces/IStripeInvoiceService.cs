using DataLayer.Entities.Orders;

namespace Core.Domain.StripePayments.Interfaces;

public interface IStripeInvoiceService
{
    StripeInvoice CreateInvoiceForSubscription(string stripePriceId, string subscriptionId, string stripeCustomerId);
}
