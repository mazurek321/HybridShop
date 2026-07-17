namespace HybridShop.Services.Order.Core.Models.Dto;

public class ProductVariantExternalDto
{
    public Guid SkuId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}