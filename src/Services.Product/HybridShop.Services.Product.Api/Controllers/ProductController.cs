
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;
using Microsoft.AspNetCore.Authorization;
using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Application.Services;
using HybridShop.Services.Product.Application.Exceptions;



namespace HybridShop.Services.Product.Api.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    private readonly IUserContext _context;
    private readonly ProductService _productService;
    public ProductController(
        IUserContext userContext,
        ProductService productService
    )
    {
        _context = userContext;
        _productService = productService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddNewProduct([FromBody] AddNewProductDto dto)
    {
        try
        {
            var userId = _context.Id;
            var product = await _productService.AddProductAsync(dto, userId);
            return Ok(product); 
        }
        catch (Exception ex) 
        {
            return BadRequest(new { message = ex.Message, StackTrace = ex.StackTrace });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProduct([FromQuery] Guid productId)
    {
        try
        {
            var product = await _productService.GetProductAsync(productId);
            return Ok(product); 
        }
        catch (ProductNotFoundException ex) 
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("browse")]
    public async Task<IActionResult> BrowseProducts([FromQuery] int skip = 0, int take = 10)
    {
        try
        {
            var products = await _productService.BrowseProductsAsync(skip, take);
            return Ok(products); 
        }
        catch (Exception ex) 
        {
            return BadRequest(new { message = ex.Message, details = ex.InnerException?.Message, stack = ex.StackTrace });
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromQuery] Guid ProductId, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var userId = _context.Id;
            var userRole = _context.Role;
            await _productService.UpdateProduct(ProductId, userId, userRole, dto);
            return Ok();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new {message = ex.Message });
        }
        catch (DontHavePermissionsException ex)
        {
            return BadRequest(new {message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteProduct([FromQuery] Guid ProductId)
    {
        try
        {
            var userId = _context.Id;
            var userRole = _context.Role;
            await _productService.DeleteProduct(ProductId, userId, userRole);
            return NoContent();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new {message = ex.Message });
        }
        catch (DontHavePermissionsException ex)
        {
            return BadRequest(new {message = ex.Message });
        }
    }
    
}