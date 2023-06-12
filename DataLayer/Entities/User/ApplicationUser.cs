using DataLayer.Entities.Orders;
using Microsoft.AspNetCore.Identity;

namespace DataLayer.Entities.User;

public class ApplicationUser
{
    public int Id { get; set; }
    public string Email { get; set; }
    public UserDocument? DriversLicense { get; set; }
    public UserDocument? IdentificationCard { get; set; }
    public List<StripeSubscription> StripeSubscriptions { get; set; }
    public List<Order> Orders { get; set; }
    public bool Approved { get; set; }
}
