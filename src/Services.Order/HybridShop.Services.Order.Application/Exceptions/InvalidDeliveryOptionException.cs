namespace HybridShop.Services.Order.Application.Exceptions;

public class InvalidDeliveryOptionException : Exception
{
    public InvalidDeliveryOptionException()  
        : base($"Delivery type is invalid.")
    {}
}