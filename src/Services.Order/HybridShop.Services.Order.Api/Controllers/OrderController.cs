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
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto dto)
    {
        try
        {
            var userId = _context.Id;
            var orders = await _orderService.CreateOrdersFromCartAsync(userId, dto);
            return Ok(orders);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = _context.Id;
        var orders = await _orderService.GetBuyerOrdersAsync(userId);
        return Ok(orders);
    }

    [Authorize]
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesOrders()
    {
        var sellerId = _context.Id;
        var orders = await _orderService.GetSellerOrdersAsync(sellerId);
        return Ok(orders);
    }

    [Authorize]
    [HttpPatch("{orderId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            if (!Enum.IsDefined(typeof(OrderStatus), dto.Status))
            {
                return BadRequest(new { message = "Nieprawidłowy status." });
            }

            await _orderService.UpdateOrderStatusAsync(orderId, (OrderStatus)dto.Status);
            return Ok();
        }
        catch (OrderNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}