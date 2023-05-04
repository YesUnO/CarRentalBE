
namespace Core.ControllerModels.User
{
    public class UserForAdminModel
    {
        public string Email { get; set; }
        public bool HasEmailVerified { get; set; }
        public string? DrivingLicenseImgBack { get; set; }
        public string? DrivingLicenseImgFront { get; set; }
        public bool HasDrivingLicenseVerified { get; set; }
        public string? IdCardImgBack { get; set; }
        public string? IdCardImgFront { get; set; }
        public bool HasIdCardVerified { get; set; }
        public bool HasActivePaymentCard { get; set; }
        public bool IsApprooved { get; set; }
    }
}
