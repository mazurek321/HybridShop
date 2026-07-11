namespace HybridShop.Services.Auth.Application.Exceptions;

public class InvalidTokenException : Exception
{
    public InvalidTokenException() 
        : base($"Token is invalid.")
    {}
}