namespace HybridShop.Services.Order.Application.Dto;

public class DeliveryOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}