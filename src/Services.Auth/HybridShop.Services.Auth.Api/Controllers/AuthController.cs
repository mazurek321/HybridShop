using HybridShop.Services.Auth.Application.Services;
using HybridShop.Services.Auth.Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;


namespace HybridShop.Services.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserContext _context;
    private readonly AuthService _authService;
    private readonly UserService _userService;

    public AuthController(
        IUserContext context,
        AuthService authService,
        UserService userService
    )
    {
        _context = context;
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var response = await _authService.RefreshAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _authService.LogoutAsync(_context.Id);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUserData()
    {
        try
        {
            var response = await _userService.GetUserDataAsync(_context.Id);
            return Ok(response);
        }
        catch(Exception)
        {
            return NotFound();
        }
    }
}