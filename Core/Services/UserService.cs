using Core.Helpers;
using Core.Services.Interfaces;
using DataLayer;
using DTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public UserService(UserManager<IdentityUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> DeleteUser(IdentityUser user)
        {
            var logins = await _userManager.GetLoginsAsync(user);
            var rolesForUser = await _userManager.GetRolesAsync(user);

            using (var transaction = _applicationDbContext.Database.BeginTransaction())
            {
                foreach (var login in logins.ToList())
                {
                    var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                    if (result.Succeeded) return false;
                }

                await _applicationDbContext.Roles.FindAsync(2);

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        // item should be the name of the role
                        var result = await _userManager.RemoveFromRoleAsync(user, item);
                        if (result.Succeeded) return false;
                    }
                }

                await _userManager.DeleteAsync(user);
                transaction.Commit();
            }
            return true;
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
