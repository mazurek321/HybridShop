
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.OpenApi.Context;
using Microsoft.AspNetCore.Authorization;
using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;



namespace HybridShop.Services.Product.Api.Controllers;

[ApiController]
[Route("api/products")]
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
    [HttpPost("create")]
    public async Task<IActionResult> AddNewProduct([FromBody] AddNewProductDto dto)
    {
        return Ok("Produkt dodany");
    }
    
}