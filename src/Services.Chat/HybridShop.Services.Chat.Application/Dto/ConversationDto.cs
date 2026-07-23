namespace HybridShop.Services.Chat.Application.Dto;

public record ConversationDto(
    Guid Id,
    Guid ParticipantOneId,
    Guid ParticipantTwoId,
    string Title,
    DateTime LastMessageAt
);