namespace HybridShop.Services.Auth.Application.Exceptions;

public class InvalidRangeException : Exception
{
    public InvalidRangeException() 
        : base($"Invalid range of users to browse.")
    {}
}