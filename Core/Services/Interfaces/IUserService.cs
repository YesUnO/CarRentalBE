using DTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUser(ClaimsPrincipal claimsPrincipal);
        List<UserDTO> GetUsers();
        Task<bool> DeleteUser(IdentityUser user);
        Task<bool> SoftDeleteUser(IdentityUser user);
        Task<bool> RegisterCustomer(IdentityUser user, string email, string password);
    }
}
