namespace HybridShop.Services.Order.Application.Exceptions;

public class InvalidDeliveryAddressException : Exception
{
    public InvalidDeliveryAddressException()  
        : base($"Delivery address is invalid.")
    {}
}