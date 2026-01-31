using DataAccess.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class ChatRepository : IChatRepository
{
    private readonly VehicleMarketContext _context;

    public ChatRepository(VehicleMarketContext context)
    {
        _context = context;
    }

    public Task<Conversation?> GetConversationAsync(int buyerId, int sellerId)
    {
        return _context.Conversations.FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.SellerId == sellerId);
    }

    public Task<Conversation?> GetConversationByIdAsync(int id)
    {
        return _context.Conversations.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddConversationAsync(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
    }

    public Task<List<Conversation>> GetConversationsForUserAsync(int userId)
    {
        return _context.Conversations
            .Where(c => c.BuyerId == userId || c.SellerId == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();
    }

    public Task<List<Message>> GetMessagesForConversationAsync(int conversationId)
    {
        return _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}
