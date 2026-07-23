namespace HybridShop.Services.Chat.Application.Exceptions;

public class InvalidRangeException : Exception
{
    public InvalidRangeException() 
        : base("Invalid pagination range.")
    {
    }
}