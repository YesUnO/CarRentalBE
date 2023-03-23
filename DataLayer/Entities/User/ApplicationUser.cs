using Microsoft.AspNetCore.Identity;

namespace DataLayer.Entities.User
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public IdentityUser IdentityUser { get; set; }
    }
}
