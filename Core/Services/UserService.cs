using Core.Helpers;
using Core.Services.Interfaces;
using DTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDTO> GetUser(ClaimsPrincipal claimsPrincipal)
        {
            var identity = await _userManager.GetUserAsync(claimsPrincipal);
            return UserHelper.GetUserFromIdentity(identity);
        }

        public List<UserDTO> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
