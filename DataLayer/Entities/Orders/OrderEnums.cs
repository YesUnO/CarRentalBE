
namespace DataLayer.Entities.Orders;

public enum Currency
{
    USD = 0,
    EUR = 1,
    CZK = 2,
}

//TODO: maybe?
public enum OrderState
{
    Set = 0,

}
public enum StripeInvoiceStatus
{
    draft = 0,
    open = 1,
    empty = 2,
    paid = 3,
    uncollectible = 4
}
