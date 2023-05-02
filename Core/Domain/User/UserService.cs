﻿using Core.ControllerModels.User;
using Core.Domain.Helpers;
using Core.Infrastructure.Emails;
using DataLayer;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Web;

namespace Core.Domain.User;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<UserService> _logger;
    private readonly IEmailService _emailService;

    public UserService(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext applicationDbContext,
            ILogger<UserService> logger
,
            IEmailService emailService)
    {
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _emailService = emailService;
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


    public async Task<bool> RegisterCustomer(IdentityUser user, string email, string password, string baseUrl)
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
                await SendConfirmationMailAsync(user, baseUrl);
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
        try
        {
            var applicationUserIdentityUserRoleJoin = _applicationDbContext.ApplicationUsers
            .Join(_applicationDbContext.UserRoles,
            user => user.IdentityUser.Id,
            userRole => userRole.UserId,
            (user, userRole) => new { User = user, UserRole = userRole });

            var userRoleNameJoin = applicationUserIdentityUserRoleJoin
                .Join(_applicationDbContext.Roles,
                userRole => userRole.UserRole.RoleId,
                role => role.Id,
                (userRole, role) => new { User = userRole.User, RoleNameNormalized = role.NormalizedName });

            var applicationUsersList = await userRoleNameJoin
                .Where(x => x.RoleNameNormalized == "CUSTOMER")
                .Select(x => x.User)
                .Include(x => x.DriversLicense)
                .Include(x => x.IdentityUser)
                .Include(x => x.IdentificationCard)
                .Include(x => x.StripeSubscriptions)
                .ToListAsync();

            return applicationUsersList.Select(x => UserHelper.GetUserResponseModelFromApplicationUser(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Retrieving customer list from Db failed");
            throw;
        }

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

    private async Task SendConfirmationMailAsync(IdentityUser user, string baseUrl)
    {
        var subject = "Account confirmation";
        var confirmAccountToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenEncoded = HttpUtility.UrlEncode(confirmAccountToken);
        var link = $"{baseUrl}?token={tokenEncoded}&email={user.Email}";
        var body = $"Hello {user.UserName}," +
            $"<p>Confirm your mail address with this link: <a href=\"{link}\">confirm mail link</a></p>";
        await _emailService.SendEmailAsync(user.Email, subject, body);
    }

}
