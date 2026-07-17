namespace HybridShop.Services.Order.Application.Dto;

public class ShoppingCartDto
{
    public Guid UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal DeliveryCost { get; set; }
    public string? DeliveryName { get; set; }
    public decimal Total { get; set; }
}