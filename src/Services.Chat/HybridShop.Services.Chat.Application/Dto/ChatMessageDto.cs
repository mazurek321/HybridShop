namespace HybridShop.Services.Chat.Application.Dto;

public record ChatMessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    Guid ReceiverId,
    string Content,
    DateTime CreatedAt,
    bool IsRead
);