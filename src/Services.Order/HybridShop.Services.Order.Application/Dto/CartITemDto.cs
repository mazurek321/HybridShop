namespace HybridShop.Services.Order.Application.Dto;

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public Guid? SkuId { get; set; }
    public int Quantity { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid SellerId { get; set; }
}