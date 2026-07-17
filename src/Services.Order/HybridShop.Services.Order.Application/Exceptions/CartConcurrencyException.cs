namespace HybridShop.Services.Order.Application.Exceptions;

public class CartConcurrencyException : Exception
{
    public CartConcurrencyException()  
        : base($"Cart version doesnt match the version in database.")
    {}
}