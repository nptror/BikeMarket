using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/chat")]
public class ChatApiController : ControllerBase
{
    private readonly VehicleMarketContext _context;

    public ChatApiController(VehicleMarketContext context)
    {
        _context = context;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var data = await _context.Conversations
            .Where(c => c.BuyerId == userId || c.SellerId == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .Select(c => new {
                c.Id,
                c.LastMessageAt
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("messages/{conversationId}")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        var data = await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .Select(m => new {
                m.Id,
                m.SenderId,
                m.Content,
                m.SentAt
            })
            .ToListAsync();

        return Ok(data);
    }
}
