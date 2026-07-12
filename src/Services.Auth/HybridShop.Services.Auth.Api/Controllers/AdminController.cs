
using HybridShop.Services.Auth.Application.Dto;
using HybridShop.Services.Auth.Application.Exceptions;
using HybridShop.Services.Auth.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridShop.Services.Auth.Api.Controllers;


[ApiController]
[Route("api/user/admin")]
public class AdminController : ControllerBase
{
    private readonly UserService _userService;

    public AdminController(
        UserService userService
    )
    {
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
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

    [Authorize(Roles = "Admin")]
    [HttpGet("browse")]
    public async Task<IActionResult> BrowseUsers([FromQuery] int skip = 0, int take = 10)
    {
        try
        {
            var data = await _userService.AdminBrowseUsersAsync(skip, take);

            return Ok(data);
        }
        catch(InvalidRangeException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromQuery] Guid? UserId, string? Email, UpdateUserDto request)
    {
        try
        {
            await _userService.AdminUpdateUserAsync(UserId, Email, request);
            return Ok();
        }
        catch(InvalidInputDataException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid? UserId, string? Email)
    {
        try
        {
            await _userService.AdminSoftDeleteUserAsync(UserId, Email);
            return NoContent();
        }
        catch(InvalidInputDataException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("ban")]
    public async Task<IActionResult> BanUser([FromQuery] Guid? UserId, string? Email)
    {
        try
        {
            await _userService.AdminBanUser(UserId, Email);
            return NoContent();
        }
        catch(UserAlreadyBannedException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("unban")]
    public async Task<IActionResult> UnbanUser([FromQuery] Guid? UserId, string? Email)
    {
        try
        {
            await _userService.AdminUnbanUser(UserId, Email);
            return NoContent();
        }
        catch(UserIsNotBannedException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
