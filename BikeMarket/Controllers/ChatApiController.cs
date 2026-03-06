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

        return Ok(data.Select(c =>
        {
            var isBuyer = c.BuyerId == userId;
            var otherUser = isBuyer ? c.Seller : c.Buyer;
            var unreadCount = isBuyer ? c.BuyerUnreadCount : c.SellerUnreadCount;

            return new
            {
                c.Id,
                OtherUserId = otherUser?.Id,
                OtherUserName = otherUser?.Name ?? "Người dùng",
                OtherUserInitial = otherUser?.Name?.FirstOrDefault().ToString().ToUpperInvariant() ?? "U",
                LastMessage = c.LastMessage?.Content,
                c.LastMessageAt,
                UnreadCount = unreadCount
            };
        }));
    }

    [HttpGet("messages/{conversationId}")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        var data = await _chatService.GetMessagesAsync(conversationId);

        return Ok(data.Select(m => new
        {
            m.Id,
            m.SenderId,
            m.Content,
            m.SentAt
        }));
    }
}
