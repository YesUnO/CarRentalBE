using Microsoft.AspNetCore.Identity;

namespace DataLayer.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public IdentityUser IdentityUser { get; set; }
    }
}
