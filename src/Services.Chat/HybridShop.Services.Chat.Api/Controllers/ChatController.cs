using BuildingBlocks.OpenApi.Context;
using HybridShop.Services.Chat.Application.Dto;
using HybridShop.Services.Chat.Application.Exceptions;
using HybridShop.Services.Chat.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridShop.Services.Chat.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IUserContext _context;
    private readonly ChatService _chatService;

    public ChatController(IUserContext context, ChatService chatService)
    {
        _context = context;
        _chatService = chatService;
    }

    [Authorize]
    [HttpPost("conversations")]
    public async Task<IActionResult> GetOrCreateConversation([FromBody] StartConversationDto dto, CancellationToken cancellationToken)
    {
        var userId = _context.Id;
        var conversation = await _chatService.GetOrCreateConversationAsync(userId, dto, cancellationToken);
        return Ok(conversation);
    }

    [Authorize]
    [HttpGet("conversations")]
    public async Task<IActionResult> GetUserConversations([FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _context.Id;
            var conversations = await _chatService.GetUserConversationsAsync(userId, skip, take, cancellationToken);
            return Ok(conversations);
        }
        catch (InvalidRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var senderId = _context.Id;
            var result = await _chatService.SendMessageAsync(senderId, dto, cancellationToken);
            return Ok(result);
        }
        catch (EmptyMessageException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConversationNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("conversations/{conversationId:guid}")]
    public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = await _chatService.GetConversationMessagesAsync(conversationId, skip, take, cancellationToken);
            return Ok(messages);
        }
        catch (InvalidRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("conversations/{conversationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid conversationId, CancellationToken cancellationToken)
    {
        var readerId = _context.Id;
        await _chatService.MarkMessagesAsReadAsync(conversationId, readerId, cancellationToken);
        return NoContent();
    }
}