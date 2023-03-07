using DTO;
using Microsoft.AspNetCore.Identity;

namespace Core.Helpers
{
    public static class UserHelper
    {
        public static UserDTO GetUserFromIdentity(IdentityUser identityUser)
        {
            return new UserDTO { Email = identityUser.Email, Name = identityUser.UserName };
        }
    }
}
