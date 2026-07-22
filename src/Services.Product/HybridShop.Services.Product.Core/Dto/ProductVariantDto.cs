namespace HybridShop.Services.Product.Core.Dto;

public class ProductVariantDto
{
    public Guid SkuId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Dictionary<string, object> Attributes { get; set; } = null!;
    public List<string> Images { get; set; } = new();
}