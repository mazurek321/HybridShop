using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;
using Microsoft.AspNetCore.Authorization;
using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Application.Services;
using HybridShop.Services.Product.Application.Exceptions;
using HybridShop.Services.Product.Core.Exceptions;

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
    public async Task<IActionResult> AddNewProduct([FromBody] AddNewProductDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            var product = await _productService.AddProductAsync(dto, userId, cancellationToken);
            return Ok(product); 
        }
        catch (InvalidProductException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex) 
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    [HttpGet("{ProductId:guid}")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid ProductId, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.GetProductAsync(ProductId, cancellationToken);
            return Ok(product); 
        }
        catch (ProductNotFoundException ex) 
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    [HttpGet("browse")]
    public async Task<IActionResult> BrowseProducts([FromQuery] BrowseProductsQueryDto query, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productService.BrowseProductsAsync(query, cancellationToken);
            return Ok(products); 
        }
        catch (Exception ex) 
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{ProductId:guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid ProductId, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            var userRole = _context.Role;
            await _productService.UpdateProduct(ProductId, userId, userRole, dto, cancellationToken);
            return Ok();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DontHavePermissionsException ex)
        {
            return Forbid();
        }
        catch (InvalidProductException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{ProductId:guid}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid ProductId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            var userRole = _context.Role;
            await _productService.DeleteProduct(ProductId, userId, userRole, cancellationToken);
            return NoContent();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DontHavePermissionsException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }
}