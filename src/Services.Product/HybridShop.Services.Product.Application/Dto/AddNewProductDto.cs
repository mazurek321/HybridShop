using HybridShop.Services.Product.Core.Product;

namespace HybridShop.Services.Product.Application.Dto;

public record AddNewProductDto(
    string Title,
    string? Description,
    decimal Price,
    int Quantity
);