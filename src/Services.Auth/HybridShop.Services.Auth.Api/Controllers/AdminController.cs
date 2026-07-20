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

    public AdminController(UserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetUserData([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _userService.AdminGetUserDataAsync(userId, cancellationToken);
            return Ok(data);
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }  

    [Authorize(Roles = "Admin")]
    [HttpGet("browse")]
    public async Task<IActionResult> BrowseUsers([FromQuery] int skip = 0, [FromQuery] int take = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await _userService.AdminBrowseUsersAsync(skip, take, cancellationToken);
            return Ok(data);
        }
        catch(InvalidRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromQuery] Guid? UserId, [FromQuery] string? Email, [FromBody] UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _userService.AdminUpdateUserAsync(UserId, Email, request, cancellationToken);
            return Ok();
        }
        catch(InvalidInputDataException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid? UserId, [FromQuery] string? Email, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.AdminSoftDeleteUserAsync(UserId, Email, cancellationToken);
            return NoContent();
        }
        catch(InvalidInputDataException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("ban")]
    public async Task<IActionResult> BanUser([FromQuery] Guid? UserId, [FromQuery] string? Email, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.AdminBanUser(UserId, Email, cancellationToken);
            return NoContent();
        }
        catch(UserAlreadyBannedException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("unban")]
    public async Task<IActionResult> UnbanUser([FromQuery] Guid? UserId, [FromQuery] string? Email, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.AdminUnbanUser(UserId, Email, cancellationToken);
            return NoContent();
        }
        catch(UserIsNotBannedException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}