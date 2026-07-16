namespace HybridShop.Services.Order.Application.Exceptions;

public class CartNotFoundException : Exception
{
    public CartNotFoundException()  
        : base($"Cart for this user not found.")
    {}
}