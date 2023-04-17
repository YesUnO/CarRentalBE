using DataLayer.Entities.User;

namespace DataLayer.Entities.Orders;

public class StripeInvoice
{
    public int Id { get; set; }
    public string? StripeInvoiceId { get; set; }
    //public StripeSubscription PaymentCard { get; set; }
    public long AmountPaid { get; set; }
    public long AmountDue { get; set; }
    public StripeInvoiceStatus StripeInvoiceStatus { get; set; }
    public Order Order { get; set; }
}
