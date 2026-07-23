namespace HybridShop.Services.Chat.Application.Exceptions;

public class EmptyMessageException : Exception
{
    public EmptyMessageException() 
        : base("Message content cannot be empty.")
    {
    }
}