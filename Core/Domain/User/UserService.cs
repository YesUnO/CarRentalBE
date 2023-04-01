using Core.Domain.Helpers;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.User;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Core.Domain.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<IdentityUser> userManager,
                           ApplicationDbContext applicationDbContext,
                           IHttpContextAccessor httpContextAccessor,
                           ILogger<UserService> logger)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
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

        public ApplicationUser GetSignedInUser()
        {
            var email = GetEmailFromHttpContext();
            var identityUser = _userManager.FindByEmailAsync(email).Result;
            var applicationUser = _applicationDbContext.ApplicationUsers.Include(x => x.DriversLicense)
                                                                        .Include(x => x.IdentificationCard)
                                                                        .FirstOrDefault(x => x.IdentityUser == identityUser);
            if (applicationUser == null)
            {
                _logger.LogWarning("Unauthorize access?");
                return null;
            }
            return applicationUser;
        }

        public async Task<ApplicationUser> GetSignedInUserAsync()
        {
            var email = GetEmailFromHttpContext();
            var identityUser = await _userManager.FindByEmailAsync(email);
            var applicationUser = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.IdentityUser == identityUser);
            if (applicationUser == null)
            {
                _logger.LogWarning("Unauthorize access?");
                return null;
            }
            return applicationUser;
        }

        private string GetEmailFromHttpContext()
        {
            //TODO: maybe??
            if (_httpContextAccessor.HttpContext.User.Identity is null)
            {
                _logger.LogWarning("Unauthorize access?");
            }
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email); ;
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
            var applicationUser = new ApplicationUser { IdentityUser = user };
            var userAdd = await _applicationDbContext.ApplicationUsers.AddAsync(applicationUser);
            await _applicationDbContext.SaveChangesAsync();
            return userAdd.IsKeySet;
        }

        private async Task<bool> DeleteApplicationUser(IdentityUser user)
        {
            var applicationUser = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.IdentityUser == user);
            if (applicationUser == null)
            {
                return true;
            }
            var result = _applicationDbContext.ApplicationUsers.Remove(applicationUser);
            return result.IsKeySet;
        }

        public async Task<ApplicationUser> GetUserByName(string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            var applicationUser = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.IdentityUser == identityUser);
            if (applicationUser == null)
            {
                _logger.LogWarning("User doesnt exist");
                return null;
            }
            return applicationUser;
        }
    }
}
