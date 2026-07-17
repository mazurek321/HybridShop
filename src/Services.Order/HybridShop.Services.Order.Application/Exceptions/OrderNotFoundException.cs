namespace HybridShop.Services.Order.Application.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException()  
        : base($"Order not found.")
    {}
}