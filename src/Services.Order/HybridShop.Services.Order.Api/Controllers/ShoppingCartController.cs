using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;
using HybridShop.Services.Order.Application.Services;
using HybridShop.Services.Order.Application.Exceptions;
using HybridShop.Services.Order.Application.Dto;
using HybridShop.Services.Order.Core.Models.Delivery;

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
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var userId = _context.Id;
            var cart = await _shoppingCartService.GetCartAsync(userId);
            return Ok(cart);
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
    {
        try
        {
            var userId = _context.Id;
            await _shoppingCartService.AddItemToCartAsync(userId, dto);
            return Ok();
        }
        catch (InvalidQuantityException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        try
        {
            var userId = _context.Id;
            await _shoppingCartService.RemoveItemFromCartAsync(userId, productId);
            return NoContent();
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var userId = _context.Id;
            await _shoppingCartService.ClearCartAsync(userId);
            return NoContent();
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("delivery")]
    public async Task<IActionResult> SetDelivery([FromBody] SetDeliveryMethodDto dto)
    {
        try
        {
            var userId = _context.Id;
            await _shoppingCartService.SetDeliveryMethodAsync(userId, dto);
            return Ok();
        }
        catch (CartNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("delivery-options")]
    public async Task<IActionResult> GetDeliveryOptions()
    {
        var options = await _shoppingCartService.GetAvailableDeliveryMethodsAsync();
        return Ok(options);
    }
}