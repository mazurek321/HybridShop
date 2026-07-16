namespace HybridShop.Services.Order.Application.Exceptions;

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException()  
        : base($"Wrong amount of items.")
    {}
}