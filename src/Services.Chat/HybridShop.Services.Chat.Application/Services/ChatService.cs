using HybridShop.Services.Chat.Application.Dto;
using HybridShop.Services.Chat.Application.Exceptions;
using HybridShop.Services.Chat.Core.Interfaces;
using HybridShop.Services.Chat.Core.Models;

namespace HybridShop.Services.Chat.Application.Services;

public class ChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserGrpcClient _userGrpcClient;

    public ChatService(IChatRepository chatRepository, IUserGrpcClient userGrpcClient)
    {
        _chatRepository = chatRepository;
        _userGrpcClient = userGrpcClient;
    }

    public async Task<ConversationDto> GetOrCreateConversationAsync(Guid userId, StartConversationDto dto, CancellationToken cancellationToken = default)
    {
        var conversation = await _chatRepository.GetConversationAsync(userId, dto.RecipientId);

        if (conversation is null)
        {
            conversation = Conversation.Create(userId, dto.RecipientId);
            await _chatRepository.CreateConversationAsync(conversation);
        }

        var title = await _userGrpcClient.GetUserFullNameAsync(dto.RecipientId, cancellationToken);

        return new ConversationDto(
            conversation.Id,
            conversation.ParticipantOneId,
            conversation.ParticipantTwoId,
            title,
            conversation.LastMessageAt
        );
    }

    public async Task<ICollection<ConversationDto>> GetUserConversationsAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        if (skip < 0 || take < 0 || take > 100)
            throw new InvalidRangeException();

        var conversations = await _chatRepository.GetUserConversationsAsync(userId, take, skip);
        var result = new List<ConversationDto>();

        foreach (var c in conversations)
        {
            var otherUserId = c.ParticipantOneId == userId ? c.ParticipantTwoId : c.ParticipantOneId;
            var title = await _userGrpcClient.GetUserFullNameAsync(otherUserId, cancellationToken);

            result.Add(new ConversationDto(
                c.Id,
                c.ParticipantOneId,
                c.ParticipantTwoId,
                title,
                c.LastMessageAt
            ));
        }

        return result;
    }

    public async Task<ChatMessageDto> SendMessageAsync(Guid senderId, SendMessageDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new EmptyMessageException();

        var conversation = await _chatRepository.GetConversationByIdAsync(dto.ConversationId);
        if (conversation is null)
            throw new ConversationNotFoundException();

        var message = ChatMessage.NewMessage(dto.ConversationId, senderId, dto.ReceiverId, dto.Content);

        await _chatRepository.AddMessageAsync(message);
        await _chatRepository.UpdateConversationLastMessageAtAsync(dto.ConversationId, message.CreatedAt);

        return new ChatMessageDto(
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.ReceiverId,
            message.Content,
            message.CreatedAt,
            message.IsRead
        );
    }

    public async Task<ICollection<ChatMessageDto>> GetConversationMessagesAsync(Guid conversationId, int skip, int take, CancellationToken cancellationToken = default)
    {
        if (skip < 0 || take < 0 || take > 100)
            throw new InvalidRangeException();

        var messages = await _chatRepository.GetMessagesByConversationIdAsync(conversationId, take, skip);

        return messages.Select(m => new ChatMessageDto(
            m.Id,
            m.ConversationId,
            m.SenderId,
            m.ReceiverId,
            m.Content,
            m.CreatedAt,
            m.IsRead
        )).ToList();
    }

    public async Task MarkMessagesAsReadAsync(Guid conversationId, Guid readerId, CancellationToken cancellationToken = default)
    {
        await _chatRepository.MarkMessagesAsReadAsync(conversationId, readerId);
    }
}