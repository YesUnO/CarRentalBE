using Core.ControllerModels.User;
using Core.Domain.Helpers;
using DataLayer;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Domain.User;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserService> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<IdentityUser> userManager,
                       ApplicationDbContext applicationDbContext,
                       IHttpContextAccessor httpContextAccessor,
                       ILogger<UserService> logger,
                       RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _roleManager = roleManager;
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

    //TODO: cleanup
    public ApplicationUser GetSignedInUser()
    {
        return new ApplicationUser();
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

    public async Task<ApplicationUser> GetUserByMailAsync(string mail, bool includeDocuments = false)
    {
        var identityUser = await _userManager.FindByEmailAsync(mail);
        ApplicationUser applicationUser;
        if (includeDocuments)
        {
            applicationUser = await _applicationDbContext.ApplicationUsers.Include(x => x.DriversLicense.BackSideImage)
                                                                          .Include(x => x.DriversLicense.FrontSideImage)
                                                                          .Include(x => x.IdentificationCard.BackSideImage)
                                                                          .Include(x => x.IdentificationCard.FrontSideImage)
                                                                          .FirstOrDefaultAsync(x => x.IdentityUser == identityUser);
        }
        else
        {
            applicationUser = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.IdentityUser == identityUser);
        }

        if (applicationUser == null)
        {
            _logger.LogWarning("User doesnt exist");
            return null;
        }
        return applicationUser;
    }

    public async Task<UserResponseModel> GetUserDTOByMailAsync(string loggedinUserMail)
    {
        var identityUser = await _userManager.FindByEmailAsync(loggedinUserMail);
        var applicationUser = await _applicationDbContext.ApplicationUsers.Include(x => x.DriversLicense)
            .Include(x => x.IdentityUser)
            .Include(x => x.IdentificationCard)
            .Include(x => x.StripeSubscriptions)
            .FirstOrDefaultAsync(x => x.IdentityUser == identityUser);
        if (applicationUser is null)
        {
            throw new Exception("User doesnt have profile... damn");
        }
        return UserHelper.GetUserResponseModelFromApplicationUser(applicationUser);
    }

    public async Task<List<UserResponseModel>> GetCustomerListAsync()
    {
        var customerRole = _roleManager.Roles.FirstOrDefault(x=>x.NormalizedName == "CUSTOMER");
        if (customerRole is null)
        {
            throw new Exception("User role wasnt find... damn");
        }

        var applicationUsersList = await _applicationDbContext.ApplicationUsers
            .Join(_applicationDbContext.UserRoles,
            user => user.IdentityUser.Id,
            userRole => userRole.UserId,
            (user, userRole) => new { User = user, UserRole = userRole })
            .Where(x => x.UserRole.RoleId == customerRole.Id)
            .Select(x=>x.User)
            .Include(x => x.DriversLicense)
            .Include(x => x.IdentityUser)
            .Include(x => x.IdentificationCard)
            .Include(x => x.StripeSubscriptions)
            .ToListAsync();
        return applicationUsersList.Select(x=> UserHelper.GetUserResponseModelFromApplicationUser(x)).ToList();
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

}
