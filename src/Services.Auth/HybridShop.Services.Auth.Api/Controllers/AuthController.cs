using HybridShop.Services.Auth.Application.Services;
using HybridShop.Services.Auth.Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;
using HybridShop.Services.Auth.Application.Exceptions;
using HybridShop.Services.Auth.Core.Exceptions;

namespace HybridShop.Services.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserContext _context;
    private readonly AuthService _authService;

    public AuthController(
        IUserContext context,
        AuthService authService
    )
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.RegisterAsync(request, cancellationToken);
            return Ok();
        }
        catch (EmailAlreadyExistsException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch(InvalidGenderException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RefreshAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UserNotFoundException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch(InvalidTokenException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            await _authService.LogoutAsync(userId, cancellationToken);
            return Ok();
        }
        catch (UserNotFoundException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}