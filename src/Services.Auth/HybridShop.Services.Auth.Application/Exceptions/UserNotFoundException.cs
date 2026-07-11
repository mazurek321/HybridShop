namespace HybridShop.Services.Auth.Application.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() 
        : base($"User not found.")
    {}
}