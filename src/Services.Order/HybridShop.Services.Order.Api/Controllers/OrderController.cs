using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;
using HybridShop.Services.Order.Application.Services;
using HybridShop.Services.Order.Application.Dto;
using HybridShop.Services.Order.Core.Models.Order;
using HybridShop.Services.Order.Application.Exceptions;

namespace HybridShop.Services.Order.Api.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IUserContext _context;
    private readonly OrderService _orderService;

    public OrderController(IUserContext context, OrderService orderService)
    {
        _context = context;
        _orderService = orderService;
    }

    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            var orders = await _orderService.CreateOrdersFromCartAsync(userId, dto, cancellationToken);
            return Ok(orders);
        }
        catch (CartConcurrencyException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex) when (
            ex is CartIsEmptyOrDoesntExistException ||
            ex is InvalidDeliveryAddressException ||
            ex is InvalidPriceException ||
            ex is InvalidQuantityException)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Wystąpił nieoczekiwany błąd serwera.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            var orders = await _orderService.GetBuyerOrdersAsync(userId, userId, cancellationToken);
            return Ok(orders);
        }
        catch (UnauthorizedException)
        {
            return Forbid();
        }
    }

    [Authorize]
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesOrders(CancellationToken cancellationToken)
    {
        try
        {
            var sellerId = _context.Id;
            var orders = await _orderService.GetSellerOrdersAsync(sellerId, cancellationToken);
            return Ok(orders);
        }
        catch (UnauthorizedException)
        {
            return Forbid();
        }
    }

    [Authorize]
    [HttpPatch("{orderId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateOrderStatusDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.IsDefined(typeof(OrderStatus), dto.Status))
            {
                return BadRequest(new { message = "Nieprawidłowy status." });
            }

            var currentUserId = _context.Id;
            var isSeller = User.IsInRole("Seller");

            await _orderService.UpdateOrderStatusAsync(orderId, (OrderStatus)dto.Status, currentUserId, isSeller, cancellationToken);
            return Ok();
        }
        catch (OrderNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedException)
        {
            return Forbid();
        }
    }
}