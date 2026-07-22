namespace HybridShop.Services.Order.Application.Dto;

public class AddCartItemDto
{
    public Guid ProductId { get; set; }
    public Guid? SkuId { get; set; }
    public int Quantity { get; set; }
}