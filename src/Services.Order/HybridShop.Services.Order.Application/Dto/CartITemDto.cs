namespace HybridShop.Services.Order.Application.Dto;

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}