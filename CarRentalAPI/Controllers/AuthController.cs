﻿using Core.Domain.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Duende.IdentityServer;
using Core.ControllerModels.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          IUserService userService,
                          ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        try
        {
            var user = new IdentityUser(model.UserName);
            var action = Url.Action("ConfirmMail");
            var url = $"{Request.Scheme}://{Request.Host}{action}";
            await _userService.RegisterCustomer(user, model.Email, model.Password, url);
            await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            var response = await HttpContext.GetTokenAsync(IdentityServerConstants.TokenTypes.AccessToken);
            return Ok(new { Result = "Succesfully registered a new customer." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registering new user failed.");
            var errorMsg = "";
            switch (ex.Message)
            {
                case "User validation failed: DuplicateUserName.":
                    errorMsg = "Username is taken. Choose another one.";
                    break;
                case "User validation failed: InvalidEmail.":
                    errorMsg = "Email is already taken or is invalid. Select another one.";
                    break;
                default:
                    break;
            }
            return BadRequest(errorMsg);
        }

    }

    [HttpGet]
    [Route("ConfirmMail")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmMail([FromQuery] ConfirmPasswordModel model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var res = await _userManager.ConfirmEmailAsync(user, model.Token);
            var feUrl = Environment.GetEnvironmentVariable("FE");
            return Redirect($"{feUrl}/confirmEmail");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Confirming mail failed.");
            return BadRequest();
        }

    }


    [HttpGet]
    [Route("ResendConfirmationEmail")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public async Task<IActionResult> ResendConfirmationEmail()
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(loggedinUserMail);

            var action = Url.Action("ConfirmMail");
            var url = $"{Request.Scheme}://{Request.Host}{action}";
            await _userService.ResendConfirmationEmailAsync(user, url);
            return Ok(new { Result = "Succesfully send confirm email." });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Confirming mail failed.");
            return BadRequest();
        }

    }


    //public async Task<IActionResult> Login(Login model)
    //{
    //    var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);

    //    if (result.Succeeded)
    //    {
    //        var user = await _signInManager.UserManager.FindByNameAsync(model.Username);
    //        var token = GenerateToken(user);
    //        return Ok(new { token });
    //    }
    //    return Unauthorized();
    //}
    //private string GenerateToken(IdentityUser user)
    //{
    //    var claims = new[]
    //{
    //    new Claim(ClaimTypes.NameIdentifier, user.Id),
    //    new Claim(ClaimTypes.Name, user.UserName),
    //};

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"));
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(
    //        issuer: "your-website-url",
    //        audience: "your-website-url",
    //        claims: claims,
    //        expires: DateTime.Now.AddMinutes(30),
    //        signingCredentials: creds);

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}
}
