namespace DataLayer.Entities.User;

public class StripeSubscription
{
    public int Id { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? CheckoutSessionReferenceId { get; set; }
    public StripeSubscriptionStatus StripeSubscriptionStatus { get; set; }

}
