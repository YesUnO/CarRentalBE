
using Core.Domain.Helpers;
using Core.Domain.StripePayments.Interfaces;
using DataLayer.Entities.User;
using DataLayer;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Core.Domain.StripePayments;

public class StripeWebhookService : IStripeWebhookService
{
    private readonly ILogger<StripeWebhookService> _logger;
    private readonly ApplicationDbContext _applicationDbContext;

    public StripeWebhookService(ILogger<StripeWebhookService> logger, ApplicationDbContext applicationDbContext)
    {
        _logger = logger;
        _applicationDbContext = applicationDbContext;
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
                    ProcessSubscriptionDeleted(subscription);
                    break;
                case Events.InvoicePaymentSucceeded:
                    var invoice = stripeEvent.Data.Object as Invoice;
                    _logger.LogInformation($"invoice {invoice.Id} payment succesfull.");
                    ProcessInvoicePaymentSucceeded(invoice);
                    break;
                case Events.InvoicePaid:
                    var invoicePaid = stripeEvent.Data.Object as Invoice;
                    _logger.LogInformation($"invoice {invoicePaid.Id} paid.");
                    ProcessInvoicePaymentSucceeded(invoicePaid);
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

    #region Event parsers

    private void ProcessInvoicePaymentSucceeded(Invoice invoice)
    {
        var dbInvoice = _applicationDbContext.Payments.FirstOrDefault(x => x.StripeInvoiceId == invoice.Id);
        if (dbInvoice != null)
        {
            dbInvoice.StripeInvoiceStatus = StripeHelper.GetStripeInvoiceStatus(invoice.Status);
            dbInvoice.AmountDue = invoice.AmountDue;
            dbInvoice.AmountPaid = invoice.AmountPaid;
            _applicationDbContext.SaveChanges();
        }
        else
        {
            _logger.LogInformation($"invoice {invoice.Id} hasnt been found");
        }
    }

    private void ProcessSubscriptionDeleted(Subscription subscriptionFromevent)
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
            var subService = new SubscriptionService();
            var subscription = subService.Get(checkoutSession.SubscriptionId);

            var dbSubscription = _applicationDbContext.StripeSubscriptions
                .FirstOrDefault(x => x.CheckoutSessionReferenceId == checkoutSession.ClientReferenceId);
            if (dbSubscription is null)
            {
                throw new Exception("subscription hasnt been created in db yet... wtf");
            }
            if (!Enum.TryParse<StripeSubscriptionStatus>(subscription.Status, out var subscriptionStatus))
            {
                throw new Exception("couldnt parse sub status");
            }
            if (checkoutSession.SubscriptionId is null || checkoutSession.CustomerId is null)
            {
                throw new Exception("couldnt parse subscription  id from webhook");
            }
            dbSubscription.StripeSubscriptionStatus = subscriptionStatus;
            dbSubscription.StripeSubscriptionId = checkoutSession.SubscriptionId;
            dbSubscription.StripeCustomerId = checkoutSession.CustomerId;

            var customerService = new CustomerService();
            var paymentMethod = customerService.ListPaymentMethods(checkoutSession.CustomerId)
                .OrderByDescending(x => x.Created)
                .FirstOrDefault();
            if (paymentMethod is null)
            {
                throw new Exception("null payment method on sub... wtf");
            }
            subService.Update(subscription.Id, new SubscriptionUpdateOptions { DefaultPaymentMethod = paymentMethod.Id });

            _applicationDbContext.SaveChanges();
        }
    }
    #endregion
}
