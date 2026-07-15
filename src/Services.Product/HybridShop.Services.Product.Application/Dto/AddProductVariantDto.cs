namespace HybridShop.Services.Product.Application.Dto;

public class AddProductVariantDto
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Dictionary<string, object> Attributes { get; set; } = new();
}