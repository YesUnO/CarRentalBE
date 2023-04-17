using DataLayer.Entities.User;
using DTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Core.Domain.User;

public interface IUserService
{
    ApplicationUser GetSignedInUser();
    Task<ApplicationUser> GetSignedInUserAsync();
    Task<ApplicationUser> GetUserByMail(string mail, bool includeDocuments = false);
    Task<UserDTO> GetUser(ClaimsPrincipal claimsPrincipal);
    List<UserDTO> GetUsers();
    Task<bool> DeleteUser(IdentityUser user);
    Task<bool> SoftDeleteUser(IdentityUser user);
    Task<bool> RegisterCustomer(IdentityUser user, string email, string password);
}
