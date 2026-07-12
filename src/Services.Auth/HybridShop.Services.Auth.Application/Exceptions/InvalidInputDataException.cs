namespace HybridShop.Services.Auth.Application.Exceptions;

public class InvalidInputDataException : Exception
{
    public InvalidInputDataException() 
        : base($"Invalid input data.")
    {}
}