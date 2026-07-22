namespace HybridShop.Services.Order.Application.Dto;

public record OrderDto
{
    public Guid Id { get; init; }
    public Guid BuyerId { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public decimal DeliveryPrice { get; init; }
    public string DeliveryName { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public string ShippingAddress { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}