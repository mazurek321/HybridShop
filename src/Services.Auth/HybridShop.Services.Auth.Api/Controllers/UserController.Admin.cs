
using HybridShop.Services.Auth.Application.Exceptions;
using HybridShop.Services.Auth.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridShop.Services.Auth.Api.Controllers;


public partial class UserController
{
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetUserData([FromQuery] Guid userId)
    {
        try
        {
            var data = await _userService.AdminGetUserDataAsync(userId);

            return Ok(data);
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }   
}
