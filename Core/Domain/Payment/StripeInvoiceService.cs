using Core.Domain.Helpers;
using DataLayer.Entities.Orders;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Core.Domain.Payment;

public class StripeInvoiceService : IStripeInvoiceService
{
    private readonly ILogger<StripeInvoiceService> _logger;

    public StripeInvoiceService(ILogger<StripeInvoiceService> logger)
    {
        _logger = logger;
    }

    public StripeInvoice CreateInvoiceForSubscription(string stripePriceId, string subscriptionId, string stripeCustomerId)
    {
        var invoiceService = new InvoiceService();
        var createOptions = new InvoiceCreateOptions
        {
            Customer = stripeCustomerId,
            Subscription = subscriptionId,
        };
        var invoice = invoiceService.Create(createOptions);


        var invoiceItemService = new InvoiceItemService();
        var options = new InvoiceItemCreateOptions
        {
            Customer = stripeCustomerId,
            Subscription = subscriptionId,
            Price = stripePriceId,
            Invoice = invoice.Id,
            
        };
        var invoiceItem = invoiceItemService.Create(options);

        invoiceService.SendInvoice(invoice.Id);

        var dbInvoice = new StripeInvoice
        {
            StripeInvoiceId= invoice.Id,
            StripeInvoiceStatus = StripeHelper.GetStripeInvoiceStatus(invoice.Status),
            AmountDue = invoice.AmountDue,
            AmountPaid= invoice.AmountPaid,
        };
        return dbInvoice;
    }
}
