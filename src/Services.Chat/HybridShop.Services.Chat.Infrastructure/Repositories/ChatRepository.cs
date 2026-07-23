using HybridShop.Services.Chat.Core.Interfaces;
using HybridShop.Services.Chat.Core.Models;
using MongoDB.Driver;

namespace HybridShop.Services.Chat.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<ChatMessage> _messages;
    private readonly IMongoCollection<Conversation> _conversations;

    public ChatRepository(IMongoDatabase database)
    {
        _messages = database.GetCollection<ChatMessage>("Messages");
        _conversations = database.GetCollection<Conversation>("Conversations");

        var messageIndexKeys = Builders<ChatMessage>.IndexKeys
            .Ascending(m => m.ConversationId)
            .Descending(m => m.CreatedAt);
        _messages.Indexes.CreateOne(new CreateIndexModel<ChatMessage>(messageIndexKeys));

        var conversationIndexKeys = Builders<Conversation>.IndexKeys
            .Ascending(c => c.ParticipantOneId)
            .Ascending(c => c.ParticipantTwoId);
        _conversations.Indexes.CreateOne(new CreateIndexModel<Conversation>(conversationIndexKeys));
    }

    public async Task AddMessageAsync(ChatMessage message)
    {
        await _messages.InsertOneAsync(message);
    }

    public async Task<List<ChatMessage>> GetMessagesByConversationIdAsync(Guid conversationId, int limit = 50, int skip = 0)
    {
        return await _messages
            .Find(m => m.ConversationId == conversationId)
            .SortByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task MarkMessagesAsReadAsync(Guid conversationId, Guid readerId)
    {
        var filter = Builders<ChatMessage>.Filter.And(
            Builders<ChatMessage>.Filter.Eq(m => m.ConversationId, conversationId),
            Builders<ChatMessage>.Filter.Eq(m => m.ReceiverId, readerId),
            Builders<ChatMessage>.Filter.Eq(m => m.IsRead, false)
        );

        var update = Builders<ChatMessage>.Update.Set(m => m.IsRead, true);

        await _messages.UpdateManyAsync(filter, update);
    }

    public async Task<Conversation?> GetConversationAsync(Guid participantOneId, Guid participantTwoId)
    {
        var sorted = new[] { participantOneId, participantTwoId }.OrderBy(p => p).ToArray();

        var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.Eq(c => c.ParticipantOneId, sorted[0]),
            Builders<Conversation>.Filter.Eq(c => c.ParticipantTwoId, sorted[1])
        );

        return await _conversations.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Conversation?> GetConversationByIdAsync(Guid conversationId)
    {
        return await _conversations.Find(c => c.Id == conversationId).FirstOrDefaultAsync();
    }

    public async Task<List<Conversation>> GetUserConversationsAsync(Guid userId, int limit = 50, int skip = 0)
    {
        var filter = Builders<Conversation>.Filter.Or(
            Builders<Conversation>.Filter.Eq(c => c.ParticipantOneId, userId),
            Builders<Conversation>.Filter.Eq(c => c.ParticipantTwoId, userId)
        );

        return await _conversations
            .Find(filter)
            .SortByDescending(c => c.LastMessageAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task CreateConversationAsync(Conversation conversation)
    {
        await _conversations.InsertOneAsync(conversation);
    }

    public async Task UpdateConversationLastMessageAtAsync(Guid conversationId, DateTime lastMessageAt)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversationId);
        var update = Builders<Conversation>.Update.Set(c => c.LastMessageAt, lastMessageAt);

        await _conversations.UpdateOneAsync(filter, update);
    }
}