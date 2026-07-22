using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;
using Microsoft.AspNetCore.Authorization;
using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Application.Services;
using HybridShop.Services.Product.Application.Exceptions;
using HybridShop.Services.Product.Core.Exceptions;

namespace HybridShop.Services.Product.Api.Controllers;

public record DeleteImageDto(string ImageUrl);

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

    [Authorize]
[HttpPost("{id:guid}/images")]
public async Task<IActionResult> UploadProductImages(
    Guid id, 
    IFormFileCollection file, 
    CancellationToken cancellationToken)
{
    if (file is null || !file.Any())
    {
        return BadRequest("Brak plików do przesłania.");
    }

    try
    {
        var userId = _context.Id;
        var imageUrls = await _productService.AddImagesToProductAsync(userId, id, file.ToList(), cancellationToken);
        return Ok(new { ImageUrls = imageUrls });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (ProductNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (DontHavePermissionsException)
    {
        return Forbid();
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
    }
}

[Authorize]
[HttpPost("{id:guid}/variants/{skuId:guid}/images")]
public async Task<IActionResult> UploadVariantImages(
    Guid id, 
    Guid skuId, 
    IFormFileCollection file, 
    CancellationToken cancellationToken)
{
    if (file is null || !file.Any())
    {
        return BadRequest("Brak plików do przesłania.");
    }

    try
    {
        var userId = _context.Id;
        var imageUrls = await _productService.AddImagesToVariantAsync(userId, id, skuId, file.ToList(), cancellationToken);
        return Ok(new { ImageUrls = imageUrls });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (ProductNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (DontHavePermissionsException)
    {
        return Forbid();
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
    }
}

    [Authorize]
    [HttpDelete("{id:guid}/images")]
    public async Task<IActionResult> DeleteProductImage(Guid id, [FromBody] DeleteImageDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            await _productService.DeleteProductImageAsync(userId, id, dto.ImageUrl, cancellationToken);
            return NoContent();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DontHavePermissionsException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}/variants/{skuId:guid}/images")]
    public async Task<IActionResult> DeleteVariantImage(Guid id, Guid skuId, [FromBody] DeleteImageDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _context.Id;
            await _productService.DeleteVariantImageAsync(userId, id, skuId, dto.ImageUrl, cancellationToken);
            return NoContent();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DontHavePermissionsException)
        {
            return Forbid();
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
        catch (DontHavePermissionsException)
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
        catch (DontHavePermissionsException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }
}