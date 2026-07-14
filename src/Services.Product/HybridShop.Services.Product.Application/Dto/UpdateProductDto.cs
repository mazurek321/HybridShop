namespace HybridShop.Services.Product.Application.Dto;

public record UpdateProductDto(
    string Title,
    string? Description,
    decimal Price,
    int Quantity
);