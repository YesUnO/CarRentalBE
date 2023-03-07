using DTO;
using System.Security.Claims;

namespace Core.Services.Interfaces
{
    internal interface IUserService
    {
        Task<UserDTO> GetUser(ClaimsPrincipal claimsPrincipal);
        List<UserDTO> GetUsers();
    }
}
