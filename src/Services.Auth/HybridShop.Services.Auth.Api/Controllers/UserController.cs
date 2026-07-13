using HybridShop.Services.Auth.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;
using HybridShop.Services.Auth.Application.Exceptions;
using HybridShop.Services.Auth.Application.Dto;
using HybridShop.Services.Auth.Core.Exceptions;


namespace HybridShop.Services.Auth.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserContext _context;
    private readonly UserService _userService;

    public UserController(
        IUserContext context,
        UserService userService
    )
    {
        _context = context;
        _userService = userService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUserData([FromQuery] Guid? userId)
    {
        try
        {
            var id = userId.HasValue ? userId.Value : _context.Id;
            var data = await _userService.GetUserDataAsync(id);

            return Ok(data);
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("browse")]
    public async Task<IActionResult> BrowseUsers([FromQuery] int skip = 0, int take = 10)
    {
        try
        {        
            var users = await _userService.BrowseUsersAsync(skip, take);
            return Ok(users);
        }
        catch(InvalidRangeException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
    {
        try
        {
            var userId = _context.Id;
            await _userService.UpdateUserAsync(userId, dto); 
            return Ok();
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new {message = ex.Message});
        }
        catch(InvalidGenderException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete()
    {
        try
        {
            var userId = _context.Id;
            await _userService.SoftDeleteUserAsync(userId);

            return NoContent();
        }
        catch(UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch(UserAlreadyDeletedException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

}