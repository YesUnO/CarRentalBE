using DataLayer.Entities.Orders;

namespace Core.Domain.Helpers
{
    public static class StripeHelper
    {
        public static StripeInvoiceStatus GetStripeInvoiceStatus(string status) => status switch
        {
            "paid" => StripeInvoiceStatus.paid,
            "open" => StripeInvoiceStatus.open,
            "draft" => StripeInvoiceStatus.draft,
            "void" => StripeInvoiceStatus.empty,
            "uncollectible" => StripeInvoiceStatus.uncollectible,
            _ => throw new NotImplementedException("Unknown invoice state"),
        };

    }
}
