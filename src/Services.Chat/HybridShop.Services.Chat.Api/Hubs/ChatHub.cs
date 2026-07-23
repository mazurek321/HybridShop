using BuildingBlocks.OpenApi.Context;
using HybridShop.Services.Chat.Core.Interfaces;
using HybridShop.Services.Chat.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HybridShop.Services.Chat.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserContext _context;

    public ChatHub(IChatRepository chatRepository, IUserContext context)
    {
        _chatRepository = chatRepository;
        _context = context;
    }

    public async Task SendMessage(Guid conversationId, Guid receiverId, string content)
    {
        var senderId = _context.Id;

        var message = ChatMessage.NewMessage(conversationId, senderId, receiverId, content);

        await _chatRepository.AddMessageAsync(message);

        await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", message);
        await Clients.Caller.SendAsync("MessageSent", message);
    }

    public async Task MarkAsRead(Guid conversationId)
    {
        var userId = _context.Id;

        await _chatRepository.MarkMessagesAsReadAsync(conversationId, userId);
        await Clients.Group(conversationId.ToString()).SendAsync("MessagesRead", new { conversationId, readerId = userId });
    }
}