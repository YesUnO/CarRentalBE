
using Stripe;

namespace Core.Domain.StripePayments.Interfaces;

public interface IStripeWebhookService
{
    void ProcessStripeEvent(Event stripeEvent);
}
