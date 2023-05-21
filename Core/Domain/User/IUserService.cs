using Core.ControllerModels.User;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.User;

public interface IUserService
{
    ApplicationUser GetSignedInUser();
    Task<ApplicationUser> GetUserByMailAsync(string mail, bool includeDocuments = false);
    Task<bool> DeleteUser(IdentityUser user);
    Task<bool> SoftDeleteUser(IdentityUser user);
    Task RegisterCustomer(IdentityUser user, string email, string password);
    Task<UserResponseModel> GetUserDTOByMailAsync(string loggedinUserMail);
    Task<List<UserForAdminModel>> GetCustomerListAsync();
    Task ResendConfirmationEmailAsync(IdentityUser user);
    Task VerifyUserDocumentAsync(VerifyDocumentRequestModel model);
    Task ApproveCustomer(string mail);
    Task<IdentityUser> HandleExternalLoginAsync(ExternalLoginInfo info);
}
