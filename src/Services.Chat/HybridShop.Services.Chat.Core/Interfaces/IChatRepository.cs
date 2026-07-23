using HybridShop.Services.Chat.Core.Models;

namespace HybridShop.Services.Chat.Core.Interfaces;

public interface IChatRepository
{
    Task AddMessageAsync(ChatMessage message);
    Task<List<ChatMessage>> GetMessagesByConversationIdAsync(Guid conversationId, int limit = 50, int skip = 0);
    Task MarkMessagesAsReadAsync(Guid conversationId, Guid readerId);

    Task<Conversation?> GetConversationAsync(Guid participantOneId, Guid participantTwoId);
    Task<Conversation?> GetConversationByIdAsync(Guid conversationId);
    Task<List<Conversation>> GetUserConversationsAsync(Guid userId, int limit = 50, int skip = 0);
    Task CreateConversationAsync(Conversation conversation);
    Task UpdateConversationLastMessageAtAsync(Guid conversationId, DateTime lastMessageAt);
}