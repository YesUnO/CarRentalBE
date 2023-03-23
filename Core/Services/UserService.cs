using Core.Helpers;
using Core.Services.Interfaces;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.User;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                var deleteApplicationUserResult = await DeleteApplicationUser(user);

                if (!deleteApplicationUserResult)
                    return false;

                foreach (var login in logins.ToList())
                {
                    var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                    if (!result.Succeeded) return false;
                }

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        // item should be the name of the role
                        var result = await _userManager.RemoveFromRoleAsync(user, item);
                        if (!result.Succeeded) return false;
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

        public async Task<bool> RegisterCustomer(IdentityUser user, string email, string password)
        {
            using (var transaction = _applicationDbContext.Database.BeginTransaction())
            {
                var creatingUserResult = await _userManager.CreateAsync(user, password);

                if (creatingUserResult.Succeeded)
                {
                    var createApplicationUser = await CreateApplicationUserAsync(user);
                    if (!createApplicationUser)
                        return false;

                    await _userManager.AddToRoleAsync(user, "customer");
                    await _userManager.SetEmailAsync(user, email);
                }
                else
                    return false;
                transaction.Commit();
            }
            return true;
        }

        public Task<bool> SoftDeleteUser(IdentityUser user)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        private async Task<bool> CreateApplicationUserAsync(IdentityUser user)
        {
            var applicationUser = new ApplicationUser { IdentityUser= user };
            var userAdd = await _applicationDbContext.ApplicationUsers.AddAsync(applicationUser);
            await _applicationDbContext.SaveChangesAsync();
            return userAdd.IsKeySet;
        }

        private async Task<bool> DeleteApplicationUser (IdentityUser user)
        {
            var applicationUser = await _applicationDbContext.ApplicationUsers.Where(x=>x.IdentityUser == user).FirstOrDefaultAsync();
            if (applicationUser == null)
            {
                return true;
            }
            var result = _applicationDbContext.ApplicationUsers.Remove(applicationUser);
            return result.IsKeySet;
        }
    }
}
