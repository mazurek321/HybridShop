using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HybridShop.Services.Chat.Core.Models;

public class Conversation
{
    [BsonConstructor]
    public Conversation() { }

    private Conversation(
        Guid id,
        Guid participantOneId,
        Guid participantTwoId,
        DateTime lastMessageAt
    )
    {
        Id = id;
        ParticipantOneId = participantOneId;
        ParticipantTwoId = participantTwoId;
        LastMessageAt = lastMessageAt;
    }

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ParticipantOneId { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ParticipantTwoId { get; private set; }

    public DateTime LastMessageAt { get; private set; }

    public static Conversation Create(Guid participantOneId, Guid participantTwoId)
    {
        var sorted = new[] { participantOneId, participantTwoId }.OrderBy(p => p).ToArray();

        return new Conversation(
            Guid.NewGuid(),
            sorted[0],
            sorted[1],
            DateTime.UtcNow
        );
    }

    public void UpdateLastMessageAt(DateTime timestamp)
    {
        LastMessageAt = timestamp;
    }
}