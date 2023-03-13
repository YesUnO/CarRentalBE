using Microsoft.AspNetCore.Mvc;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CarRentalAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(IUserService userService, UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpDelete]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userService.DeleteUser(user);
            
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
