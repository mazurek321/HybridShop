namespace HybridShop.Services.Order.Application.Exceptions;

public class InvalidPriceException : Exception
{
    public InvalidPriceException()  
        : base($"Price of item doesnt match.")
    {}
}