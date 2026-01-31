using Business.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/chat")]
public class ChatApiController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatApiController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var data = await _chatService.GetConversationsAsync(userId);

        return Ok(data.Select(c => new {
            c.Id,
            c.LastMessageAt
        }));
    }

    [HttpGet("messages/{conversationId}")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        var data = await _chatService.GetMessagesAsync(conversationId);

        return Ok(data.Select(m => new {
            m.Id,
            m.SenderId,
            m.Content,
            m.SentAt
        }));
    }
}
