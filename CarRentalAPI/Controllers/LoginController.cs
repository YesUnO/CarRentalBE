//using CarRentalAPI.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace CarRentalAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class LoginController : ControllerBase
//    {
//        private readonly SignInManager<IdentityUser> _signInManager;

//        public LoginController(SignInManager<IdentityUser> signInManager)
//        {
//            _signInManager = signInManager;
//        }

//        public async Task<IActionResult> Login(Login model)
//        {
//            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);

//            if (result.Succeeded)
//            {
//                var user = await _signInManager.UserManager.FindByNameAsync(model.Username);
//                var token = GenerateToken(user);
//                return Ok(new { token });
//            }
//            return Unauthorized();
//        }
//        private string GenerateToken(IdentityUser user)
//        {
//            var claims = new[]
//        {
//            new Claim(ClaimTypes.NameIdentifier, user.Id),
//            new Claim(ClaimTypes.Name, user.UserName),
//        };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: "your-website-url",
//                audience: "your-website-url",
//                claims: claims,
//                expires: DateTime.Now.AddMinutes(30),
//                signingCredentials: creds);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
