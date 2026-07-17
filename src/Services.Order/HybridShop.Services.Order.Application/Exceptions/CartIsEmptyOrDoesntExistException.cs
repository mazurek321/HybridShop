namespace HybridShop.Services.Order.Application.Exceptions;

public class CartIsEmptyOrDoesntExistException : Exception
{
    public CartIsEmptyOrDoesntExistException()  
        : base($"Cart for user is empty or doesnt exist.")
    {}
}