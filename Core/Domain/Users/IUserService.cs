using Core.ControllerModels.User;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.User;

public interface IUserService
{
    ApplicationUser GetSignedInUser();
    Task<ApplicationUser> GetUserByMailAsync(string mail, bool includeDocuments = false);
    Task RegisterCustomer(string email, string password, string username);
    Task<UserResponseModel> GetUserDTOByMailAsync(string loggedinUserMail);
    Task<List<UserForAdminModel>> GetCustomerListAsync();
    Task ResendConfirmationEmailAsync(string email);
    Task VerifyUserDocumentAsync(VerifyDocumentRequestModel model);
    Task ApproveCustomer(string mail);
}
