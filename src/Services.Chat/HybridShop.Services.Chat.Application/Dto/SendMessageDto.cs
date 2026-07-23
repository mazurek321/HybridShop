namespace HybridShop.Services.Chat.Application.Dto;

public record SendMessageDto(
    Guid ConversationId,
    Guid ReceiverId,
    string Content
);