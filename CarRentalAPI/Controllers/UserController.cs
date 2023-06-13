using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Core.Domain.User;
using Core.Infrastructure.Files;
using System.Security.Claims;
using System.Drawing;
using Core.ControllerModels.User;
using CarRentalAPI.Models.User;

namespace CarRentalAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUsersService _userService;
    private readonly IFileService _fileService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<UserController> _logger;

    public UserController(IUsersService userService, UserManager<IdentityUser> userManager, IFileService fileService, ILogger<UserController> logger)
    {
        _userService = userService;
        _userManager = userManager;
        _fileService = fileService;
        _logger = logger;
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Delete([FromQuery] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest();
        }
        //TODO: handle delete
        var result = true;

        if (result)
        {
            return Ok(new
            {
                Success = true,
            });
        }
        return BadRequest();
    }



    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var result = await _userService.GetUserDTOByMailAsync(loggedinUserMail);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("List")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var result = await _userService.GetCustomerListAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("VerifyDocument")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> VerifyDoc(VerifyDocumentRequestModel model)
    {
        try
        {
            await _userService.VerifyUserDocumentAsync(model);
            return Ok(new
            {
                Success = true,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer veryfication failed");
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("Approve")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Approove(ApproveUserRequestModel model)
    {
        try
        {
            await _userService.ApproveCustomer(model.Email);
            return Ok(new
            {
                Success = true,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer approoval failed");
            return BadRequest();
        }
    }
}
