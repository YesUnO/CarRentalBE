using Core.ControllerModels.User;
using DataLayer.Entities.User;

namespace Core.Domain.Helpers;

public static class UserHelper
{
    public static UserResponseModel GetUserResponseModelFromApplicationUser(ApplicationUser applicationUser)
    {
        return new UserResponseModel
        {
            Email = applicationUser.IdentityUser.Email,
            IsApprooved = applicationUser.Approved,
            HasEmailVerified = applicationUser.IdentityUser.EmailConfirmed,
            HasDrivingLicense = applicationUser.DriversLicense != null,
            HasDrivingLicenseVerified = applicationUser.DriversLicense != null && applicationUser.DriversLicense.Checked,
            HasIdCard = applicationUser.IdentificationCard != null,
            HasIdCardVerified = applicationUser.IdentificationCard != null && applicationUser.IdentificationCard.Checked,
            HasActivePaymentCard = applicationUser.StripeSubscriptions.Any(x => x.StripeSubscriptionStatus == StripeSubscriptionStatus.active),
        };
    }
}
