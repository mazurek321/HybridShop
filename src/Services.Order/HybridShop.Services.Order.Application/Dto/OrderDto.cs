namespace HybridShop.Services.Order.Application.Dto;
public class OrderDto
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal DeliveryPrice { get; set; }
    public string DeliveryName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}