using Core.Domain.Helpers;
using Core.Domain.User;
using Core.Infrastructure.StripePayment.Options;
using DataLayer;
using DataLayer.Entities.User;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Core.Infrastructure.StripePayment;

public class StripeSubscriptionService : IStripeSubscriptionService
{
    private readonly StripeSettings _stripeSettings;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IUserService _userService;
    private readonly ILogger<StripeSubscriptionService> _logger;

    public StripeSubscriptionService(IOptions<StripeSettings> stripeSettings, ApplicationDbContext applicationDbContext, IUserService userService, ILogger<StripeSubscriptionService> logger)
    {
        _stripeSettings = stripeSettings.Value;
        _applicationDbContext = applicationDbContext;
        _userService = userService;
        _logger = logger;
    }

    public string CheckCheckoutSession(string feUrl, string clientMail)
    {
        var clientReferenceId = Guid.NewGuid().ToString();
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
            ClientReferenceId = clientReferenceId,
            SuccessUrl = feUrl + "?success=true&session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = feUrl + "?canceled=true",
        };
        var service = new SessionService();
        Session session = service.Create(options);
        CreateStripeSubscriptionFromCheckoutSession(clientReferenceId, clientMail);
        return session.Url;
    }

    private StripeSubscription? GetStripeSubscription(string clientMail)
    {
        var subscription = _applicationDbContext.StripeSubscriptions.FirstOrDefault(x => x.CheckoutSessionReferenceId == clientMail);
        return subscription;
    }

    private bool CreateStripeSubscriptionFromCheckoutSession(string clientReferenceId, string clientMail)
    {
        var loggedinUser = _userService.GetUserByMail(clientMail).Result;
        var stripeSubscription = new StripeSubscription
        {
            ApplicationUser = loggedinUser,
            StripeSubscriptionStatus = StripeSubscriptionStatus.incomplete,
            CheckoutSessionReferenceId = clientReferenceId,
        };

        _applicationDbContext.Add(stripeSubscription);
        var saveToDb = _applicationDbContext.SaveChanges();
        return saveToDb != 0;
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

    private void ProcessInvoicePaymentSucceeded (Invoice invoice)
    {
        var dbInvoice = _applicationDbContext.Payments.FirstOrDefault(x => x.StripeInvoiceId == invoice.Id);
        if (dbInvoice != null) {
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

            var dbSubscription = GetStripeSubscription(checkoutSession.ClientReferenceId);
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
            var paymentMethod = customerService.ListPaymentMethods(checkoutSession.CustomerId).OrderByDescending(x=>x.Created).FirstOrDefault();
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
