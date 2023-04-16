using DataLayer.Entities.User;

namespace DataLayer.Entities.Orders;

public class Payment
{
    public int Id { get; set; }
    public StripeSubscription PaymentCard { get; set; }

    //TOFO: maybe?
    //public ApplicationUser User { get; set; }
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime CanceledAt { get; set; }
    public Order Order { get; set; }
}
