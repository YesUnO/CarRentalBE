using CarRentalIdentityServer.Models;
using Core.ControllerModels.User;
using Core.Domain.Helpers;
using Core.Exceptions.UserRegistration;
using Core.Infrastructure.Emails;
using Core.Infrastructure.Options;
using DataLayer;
using DataLayer.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
//using System.Text.Json;
using System.Web;

namespace Core.Domain.User;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<UserService> _logger;
    private readonly IEmailService _emailService;
    private readonly BaseApiUrls _baseApiUrls;
    IHttpClientFactory _clientFactory;

    public UserService(
            ApplicationDbContext applicationDbContext,
            ILogger<UserService> logger,
            IEmailService emailService,
            IOptions<BaseApiUrls> baseApiUrls,
            IHttpClientFactory factory
        )
    {
        _baseApiUrls = baseApiUrls.Value;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _emailService = emailService;
        _clientFactory = factory;
    }

    #region to be deleted

    //public async Task<bool> DeleteUser(IdentityUser user)
    //{
    //    var logins = await _userManager.GetLoginsAsync(user);
    //    var rolesForUser = await _userManager.GetRolesAsync(user);

    //    using (var transaction = _applicationDbContext.Database.BeginTransaction())
    //    {
    //        var deleteApplicationUserResult = await DeleteApplicationUser(user);

    //        if (!deleteApplicationUserResult)
    //            return false;

    //        foreach (var login in logins.ToList())
    //        {
    //            var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
    //            if (!result.Succeeded) return false;
    //        }

    //        if (rolesForUser.Count() > 0)
    //        {
    //            foreach (var item in rolesForUser.ToList())
    //            {
    //                // item should be the name of the role
    //                var result = await _userManager.RemoveFromRoleAsync(user, item);
    //                if (!result.Succeeded) return false;
    //            }
    //        }

    //        await _userManager.DeleteAsync(user);
    //        transaction.Commit();
    //    }
    //    return true;
    //}

    //public async Task<IdentityUser> HandleExternalLoginAsync(ExternalLoginInfo info)
    //{
    //    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
    //    var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
    //    if (user == null)
    //    {
    //        user = await _userManager.FindByEmailAsync(email);
    //        if (user == null)
    //        {
    //            user = await RegisterCustomerFromGoogleSignin(email, user);
    //        }
    //        await _userManager.AddLoginAsync(user, info);
    //    }
    //    return user;
    //}

    #endregion

    //TODO: cleanup
    public ApplicationUser GetSignedInUser()
    {
        return new ApplicationUser();
    }


    public async Task ApproveCustomer(string mail)
    {
        var customer = await GetUserByMailAsync(mail, true);
        if (customer == null)
        {
            _logger.LogError("Customer doesnt exist");
            throw new Exception("Customer doesnt exist");
        }

        if (
            customer.DriversLicense is null ||
            !customer.DriversLicense.Checked ||
            customer.IdentificationCard is null ||
            !customer.IdentificationCard.Checked ||
            customer.StripeSubscriptions is null ||
            !customer.StripeSubscriptions.Any(x => x.StripeSubscriptionStatus == StripeSubscriptionStatus.active)
            )
        {
            throw new Exception("Customer not ready");
        }

        customer.Approved = true;
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task RegisterCustomer(string email, string password, string username)
    {
        CheckAvailibility(username, email);
        var creatingUserResult = await RegisterCustomerOnIdentityServerAsync(email, password, username);

        if (creatingUserResult.Exception is not null)
        {
            throw creatingUserResult.Exception;
        }

        if (!creatingUserResult.Succeeded)
        {
            throw new UserRegistrationException(
                "Unable to create user",
                creatingUserResult.Errors.Select(x =>
                    new UserRegistrationError { Description = x.Description, Field = ParseIdentityErrorCodesToFields(x.Code) }));
        }

        await CreateApplicationUserAsync(email, username);

        await SendConfirmationMailAsync(email, creatingUserResult.EmailConfirmationToken, username);
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<ApplicationUser> GetUserByMailAsync(string mail, bool includeDocuments = false)
    {
        ApplicationUser? applicationUser = includeDocuments ?
            await _applicationDbContext.ApplicationUsers
            .Include(x => x.DriversLicense.BackSideImage)
            .Include(x => x.DriversLicense.FrontSideImage)
            .Include(x => x.IdentificationCard.BackSideImage)
            .Include(x => x.IdentificationCard.FrontSideImage)
            .Include(x => x.StripeSubscriptions)
            .FirstOrDefaultAsync(x => x.Email == mail) :
            await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == mail);

        if (applicationUser == null)
        {
            _logger.LogWarning("User doesnt exist");
            throw new Exception("User doesnt exist");
        }
        return applicationUser;
    }

    public async Task<UserResponseModel> GetUserDTOByMailAsync(string loggedinUserMail)
    {
        var applicationUser = await GetUserByMailAsync(loggedinUserMail, true);
        return UserHelper.GetUserResponseModelFromApplicationUser(applicationUser);
    }

    public async Task<List<UserForAdminModel>> GetCustomerListAsync()
    {
        try
        {
            return _applicationDbContext.ApplicationUsers.Select(x => UserHelper.GetUserForAdminModelFromApplicationUser(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Retrieving customer list from Db failed");
            throw;
        }

    }

    public async Task ResendConfirmationEmailAsync(string email)
    {
        var user = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == email);
        if (user is null)
        {
            throw new Exception("User doesnt exist");
        }
        var token = await GetEmailConfirmationTokenFromIdentityServer(email);
        await SendConfirmationMailAsync(email, token, user.Username);
    }

    public async Task VerifyUserDocumentAsync(VerifyDocumentRequestModel model)
    {
        var applicationUser = await GetUserByMailAsync(model.CustomerMail, true);
        if (model.UserDocumentType == UserDocumentType.DriversLicense)
        {
            if (applicationUser.DriversLicense == null)
            {
                throw new Exception("Document doesnt exist");
            }
            applicationUser.DriversLicense.DocNr = model.DocNr;
            applicationUser.DriversLicense.ValidTill = model.ValidTill;
            applicationUser.DriversLicense.Checked = true;
        }
        else
        {
            if (applicationUser.IdentificationCard == null)
            {
                throw new Exception("Document doesnt exist");
            }
            applicationUser.IdentificationCard.DocNr = model.DocNr;
            applicationUser.IdentificationCard.ValidTill = model.ValidTill;
            applicationUser.IdentificationCard.Checked = true;
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    #region private methods

    private void CheckAvailibility(string username, string email)
    {
        var errors = new List<UserRegistrationError>();
        var emailIsTaken = _applicationDbContext.ApplicationUsers.Any(x => x.Email == email);
        if (emailIsTaken)
        {
            errors.Add(new UserRegistrationError
            {
                Description = $"Email '{email}' is already taken.",
                Field = ParseIdentityErrorCodesToFields("DuplicateEmail")
            });
        }
        var usernameIsTaken = _applicationDbContext.ApplicationUsers.Any(x => x.Username == username);
        if (usernameIsTaken)
        {
            errors.Add(new UserRegistrationError
            {
                Description = $"Username '{username}' is already taken.",
                Field = ParseIdentityErrorCodesToFields("DuplicateUserName")
            });
        }
        if (errors.Any())
        {
            throw new UserRegistrationException("Unable to create user",errors);
        }
    }

    private async Task<RegisterResponseModel> RegisterCustomerOnIdentityServerAsync(string email, string password, string username)
    {
        var client = _clientFactory.CreateClient("identityServerClient");
        var requestBody = JsonConvert.SerializeObject(new { Username = username, Password = password, Email = email });
        var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync(new Uri("https://localhost:5001/api/admin"), content);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RegisterResponseModel>(body);
        return result;
    }

    private async Task<string> GetEmailConfirmationTokenFromIdentityServer(string email)
    {
        var client = _clientFactory.CreateClient("identityServerClient");

        var response = await client.GetAsync(new Uri($"https://localhost:5001/api/admin/EmailConfirmation?email={email}"));
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<string>(body);
        return result;
    }

    private string ParseIdentityErrorCodesToFields(string code) => code switch
    {
        "ConcurrencyFailure" => "concurrency",
        "DefaultError" => "default",
        "DuplicateEmail" => "email",
        "DuplicateRoleName" => "roleName",
        "DuplicateUserName" => "username",
        "InvalidEmail" => "email",
        "InvalidRoleName" => "roleName",
        "InvalidToken" => "token",
        "InvalidUserName" => "userName",
        "LoginAlreadyAssociated" => "login",
        "PasswordMismatch" => "password",
        "PasswordRequiresDigit" => "password",
        "PasswordRequiresLower" => "password",
        "PasswordRequiresNonAlphanumeric" => "password",
        "PasswordRequiresUniqueChars" => "password",
        "PasswordRequiresUpper" => "password",
        "PasswordTooShort" => "password",
        "RecoveryCodeRedemptionFailed" => "recoveryCode",
        "UserAlreadyHasPassword" => "password",
        "UserAlreadyInRole" => "roleName",
        "UserLockoutNotEnabled" => "lockout",
        "UserNotInRole" => "roleName"
    };

    private async Task CreateApplicationUserAsync(string email, string username)
    {
        var applicationUser = new ApplicationUser { Email = email, Username = username };
        var userAdd = await _applicationDbContext.ApplicationUsers.AddAsync(applicationUser);
    }


    private async Task SendConfirmationMailAsync(string email, string confirmationEmailtoken, string name)
    {
        var subject = "Account confirmation";
        var tokenEncoded = HttpUtility.UrlEncode(confirmationEmailtoken);
        var baseUrl = new Uri(_baseApiUrls.HttpsUrl + "/api/auth/ConfirmMail");
        var link = $"{baseUrl}?token={tokenEncoded}&email={email}";
        var body = $"Hello {name}," +
            $"<p>Confirm your mail address with this link: <a href=\"{link}\">confirm mail link</a></p>";
        await _emailService.SendEmailAsync(email, subject, body);
    }
    #endregion
}
