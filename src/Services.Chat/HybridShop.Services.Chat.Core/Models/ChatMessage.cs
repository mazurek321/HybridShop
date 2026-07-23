using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HybridShop.Services.Chat.Core.Models;

public class ChatMessage
{
    [BsonConstructor]
    public ChatMessage(){}
    private ChatMessage(
        Guid id,
         Guid conversationId,
        Guid senderId,
        Guid receiverId,
        string content,
        DateTime createdAt,
        bool isRead
    )
    {
        Id = id;
        ConversationId = conversationId;
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        CreatedAt = createdAt;
        IsRead = isRead;   
    }

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ConversationId { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Guid SenderId { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ReceiverId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsRead { get; private set; } = false;

    public static ChatMessage NewMessage(
        Guid conversationId,
        Guid senderId,
        Guid receiverId,
        string content
    )
    {
        return new ChatMessage(
            Guid.NewGuid(), conversationId, senderId, receiverId, content, DateTime.UtcNow, false
        );
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}