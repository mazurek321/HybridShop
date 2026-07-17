namespace HybridShop.Services.Order.Application.Dto;

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}