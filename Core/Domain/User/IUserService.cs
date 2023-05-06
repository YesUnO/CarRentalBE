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
    Task<bool> RegisterCustomer(IdentityUser user, string email, string password, string baseUrl);
    Task<UserResponseModel> GetUserDTOByMailAsync(string loggedinUserMail);
    Task<List<UserForAdminModel>> GetCustomerListAsync();
    Task ResendConfirmationEmailAsync(IdentityUser user, string confirmUrl);
    Task VerifyUserDocumentAsync(VerifyDocumentRequestModel model);
    Task ApproveCustomer(string mail);
}
