namespace HybridShop.Services.Order.Core.Models.Dto;

public class ProductExternalDto
{
    public Guid ProductId { get; set; }
    public Guid? SkuId { get; set; }
    public Guid SellerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}