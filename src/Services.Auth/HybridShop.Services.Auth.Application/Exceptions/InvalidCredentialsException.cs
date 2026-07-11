namespace HybridShop.Services.Auth.Application.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() 
        : base($"Incorrect email od password.")
    {}
}