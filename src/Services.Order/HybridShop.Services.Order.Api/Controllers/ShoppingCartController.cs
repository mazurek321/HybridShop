using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Grpc.Core;
using BuildingBlocks.OpenApi.Context;
using HybridShop.Services.Order.Application.Services;
using HybridShop.Services.Order.Application.Exceptions;
using HybridShop.Services.Order.Application.Dto;
using HybridShop.Services.Order.Core.Exceptions;

namespace HybridShop.Services.Order.Api.Controllers;

[ApiController]
[Route("api/order/shopping-cart")]
public class ShoppingCartController : ControllerBase
{
    private readonly IUserContext _context;
    private readonly ShoppingCartService _shoppingCartService;

    public ShoppingCartController(
        IUserContext context,
        ShoppingCartService shoppingCartService
    )
    {
        _context = context;
        _shoppingCartService = shoppingCartService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            var cart = await _shoppingCartService.GetCartAsync(userId, cancellationToken);
            return Ok(cart);
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(new { message = ex.Status.Detail });
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(new { message = ex.Status.Detail });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Wystąpił nieoczekiwany błąd serwera.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            await _shoppingCartService.AddItemToCartAsync(userId, dto, cancellationToken);
            return Ok();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(new { message = ex.Status.Detail });
        }
        catch (RpcException ex) when (ex.StatusCode == global::Grpc.Core.StatusCode.InvalidArgument)
        {
            return BadRequest(new { message = ex.Status.Detail });
        }
        catch (Exception ex) when (
            ex is Application.Exceptions.InvalidQuantityException ||
            ex is InvalidPriceException ||
            ex is ArgumentException)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Wystąpił nieoczekiwany błąd serwera.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            await _shoppingCartService.RemoveItemFromCartAsync(userId, productId, cancellationToken);
            return NoContent();
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Wystąpił nieoczekiwany błąd serwera.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            await _shoppingCartService.ClearCartAsync(userId, cancellationToken);
            return NoContent();
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Wystąpił nieoczekiwany błąd serwera.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("delivery")]
    public async Task<IActionResult> SetDelivery([FromBody] SetDeliveryMethodDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            await _shoppingCartService.SetDeliveryMethodAsync(userId, dto, cancellationToken);
            return Ok();
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex) when (
            ex is InvalidDeliveryOptionException ||
            ex is InvalidDeliveryTypeException)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Wystąpił nieoczekiwany błąd serwera.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("delivery-options")]
    public async Task<IActionResult> GetDeliveryOptions()
    {
        try
        {
            var options = await _shoppingCartService.GetAvailableDeliveryMethodsAsync();
            return Ok(options);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Wystąpił nieoczekiwany błąd serwera.", details = ex.Message });
        }
    }
}