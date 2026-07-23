namespace HybridShop.Services.Chat.Application.Exceptions;

public class ConversationNotFoundException : Exception
{
    public ConversationNotFoundException() 
        : base("Conversation was not found.")
    {
    }
}