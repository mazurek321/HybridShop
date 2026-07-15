using System.ComponentModel.DataAnnotations;
using HybridShop.Services.Product.Core.Product;

namespace HybridShop.Services.Product.Application.Dto;

public record AddNewProductDto{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price value is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = string.Empty;
    public Dictionary<string, object>? Attributes { get; set; } = new();
    public List<AddProductVariantDto>? Variants { get; set; } = new();
}