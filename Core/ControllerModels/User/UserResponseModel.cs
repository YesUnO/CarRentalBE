﻿
namespace Core.ControllerModels.User
{
    public class UserResponseModel
    {
        public string Email { get; set; }
        public bool HasEmailVerified { get; set; }
        public bool HasDrivingLicense { get; set; }
        public bool HasDrivingLicenseVerified { get; set; }
        public bool HasIdCard { get; set; }
        public bool HasIdCardVerified { get; set; }
        public bool HasActivePaymentCard { get; set; }
        public bool IsApprooved { get; set; }
    }
}