namespace HybridShop.Services.Order.Core.Exceptions;

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException()  
        : base($"Wrong amount of items.")
    {}
}