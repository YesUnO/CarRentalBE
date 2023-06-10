
namespace Core.ControllerModels.User
{
    public class UserResponseModel
    {
        public bool HasDrivingLicense { get; set; }
        public bool HasDrivingLicenseVerified { get; set; }
        public bool HasDrivingLicenseFrontImg { get; set; }
        public bool HasDrivingLicenseBackImg { get; set; }
        public bool HasIdFrontImg { get; set; }
        public bool HasIdBackImg { get; set; }
        public bool HasIdCard { get; set; }
        public bool HasIdCardVerified { get; set; }
        public bool HasActivePaymentCard { get; set; }
        public bool IsApprooved { get; set; }
    }
}
