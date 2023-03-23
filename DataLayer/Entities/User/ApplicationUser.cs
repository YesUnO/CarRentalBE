using Microsoft.AspNetCore.Identity;

namespace DataLayer.Entities.User
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public IdentityUser IdentityUser { get; set; }
        public UserDocument? DriversLicense { get; set; }
        public UserDocument? IdentificationCard { get; set; }
        public bool Approved { get; set; }
    }
}
