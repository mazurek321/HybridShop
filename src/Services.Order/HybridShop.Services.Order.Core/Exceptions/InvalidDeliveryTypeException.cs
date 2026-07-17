namespace HybridShop.Services.Order.Core.Exceptions;

public class InvalidDeliveryTypeException : Exception
{
    public InvalidDeliveryTypeException()  
        : base($"Invalid delivery type.")
    {}
}