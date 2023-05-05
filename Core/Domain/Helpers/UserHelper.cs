using Core.ControllerModels.User;
using DataLayer.Entities.User;

namespace Core.Domain.Helpers;

public static class UserHelper
{
    public static UserResponseModel GetUserResponseModelFromApplicationUser(ApplicationUser applicationUser) => new UserResponseModel
    {
        Email = applicationUser.IdentityUser.Email,
        IsApprooved = applicationUser.Approved,
        HasEmailVerified = applicationUser.IdentityUser.EmailConfirmed,
        HasDrivingLicense = applicationUser.DriversLicense != null,
        HasDrivingLicenseBackImg = applicationUser.DriversLicense != null && applicationUser.DriversLicense.BackSideImage != null,
        HasDrivingLicenseFrontImg = applicationUser.DriversLicense != null && applicationUser.DriversLicense.FrontSideImage != null,
        HasDrivingLicenseVerified = applicationUser.DriversLicense != null &&
                applicationUser.DriversLicense.Checked,
        HasIdCard = applicationUser.IdentificationCard != null,
        HasIdBackImg= applicationUser.IdentificationCard != null && applicationUser.IdentificationCard.BackSideImage != null,
        HasIdFrontImg= applicationUser.IdentificationCard != null && applicationUser.IdentificationCard.FrontSideImage != null,
        HasIdCardVerified = applicationUser.IdentificationCard != null &&
                applicationUser.IdentificationCard.Checked,
        HasActivePaymentCard = applicationUser.StripeSubscriptions != null &&
                applicationUser.StripeSubscriptions.Any(x => x.StripeSubscriptionStatus == StripeSubscriptionStatus.active),
    };

    public static UserForAdminModel GetUserForAdminModelFromApplicationUser(ApplicationUser applicationUser)
    {
        var email = applicationUser.IdentityUser.Email;
        return new UserForAdminModel
        {
            Email = applicationUser.IdentityUser.Email,
            IsApprooved = applicationUser.Approved,
            HasEmailVerified = applicationUser.IdentityUser.EmailConfirmed,
            HasDrivingLicenseVerified = applicationUser.DriversLicense != null && applicationUser.DriversLicense.Checked,
            HasIdCardVerified = applicationUser.IdentificationCard != null && applicationUser.IdentificationCard.Checked,
            HasActivePaymentCard = applicationUser.StripeSubscriptions.Any(x => x.StripeSubscriptionStatus == StripeSubscriptionStatus.active),
            IdCardImgBack = (applicationUser.IdentificationCard == null || applicationUser.IdentificationCard.BackSideImage == null) ?
                null :
                applicationUser.IdentificationCard.BackSideImage.RelativePath,
            IdCardImgFront = (applicationUser.IdentificationCard == null || applicationUser.IdentificationCard.FrontSideImage == null) ?
                null :
                applicationUser.IdentificationCard.FrontSideImage.RelativePath,
            DrivingLicenseImgBack = (applicationUser.DriversLicense == null || applicationUser.DriversLicense.BackSideImage == null) ?
                null :
                applicationUser.DriversLicense.BackSideImage.RelativePath,
            DrivingLicenseImgFront = (applicationUser.DriversLicense == null || applicationUser.DriversLicense.FrontSideImage == null) ?
                null :
                applicationUser.DriversLicense.FrontSideImage.RelativePath,
        };
    }
}
