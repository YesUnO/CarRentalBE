using CarRentalAPI.Models;
using CarRentalAPI.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new IdentityUser(model.UserName);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.SetEmailAsync(user, model.Email);
                await _userManager.SetEmailAsync(user, model.Email);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }
            return BadRequest();
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
}
