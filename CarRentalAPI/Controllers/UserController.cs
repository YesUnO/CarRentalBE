using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Core.Domain.User;
using Core.Infrastructure.Files;

namespace CarRentalAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFileService _fileService;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(IUserService userService, UserManager<IdentityUser> userManager, IFileService fileService)
    {
        _userService = userService;
        _userManager = userManager;
        _fileService = fileService;
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Delete(string name)
    {
        var user = await _userManager.FindByNameAsync(name);
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


    [HttpDelete]
    [Route("SoftDelete")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> SoftDelete(string name)
    {
        var user = await _userManager.FindByNameAsync(name);
        if (user == null)
        {
            return NotFound();
        }
        var result = await _userService.SoftDeleteUser(user);

        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }

    public  class TestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Test(TestModel testModel)
    {

        return Ok();
    }

    [HttpPost]
    [Route("Protected")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public async Task<IActionResult> TestProtected(TestModel testModel)
    {

        return Ok();
    }
}
