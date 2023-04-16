
namespace DataLayer.Entities.User;

public enum UserDocumentType
{
    DriversLicense = 0,
    IdentityCard = 1
}
public enum StripeSubscriptionStatus
{
    active = 0,
    past_due = 1,
    unpaid = 2,
    canceled = 3,
    incomplete = 4,
    incomplete_expired = 5,
    trialing = 6,
    paused = 7,
}
