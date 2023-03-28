﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Core.Domain.User;
using CarRentalAPI.Models.File;

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

        [HttpPost]
        [HttpPost("{orderId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ViewOwnOrdersPolicy")]
        public async Task<IActionResult> Post([FromRoute]int? orderId)
        {
            return Ok();
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin", Policy = "")]
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
    }
}
