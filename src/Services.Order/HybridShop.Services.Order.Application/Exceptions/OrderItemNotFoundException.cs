namespace HybridShop.Services.Order.Application.Exceptions;

public class OrderItemNotFoundException : Exception
{
    public OrderItemNotFoundException()  
        : base($"Item not found.")
    {}
}