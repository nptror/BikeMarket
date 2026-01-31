using DataAccess.Models;

namespace DataAccess.Interface;

public interface IChatRepository
{
    Task<Conversation?> GetConversationAsync(int buyerId, int sellerId);
    Task<Conversation?> GetConversationByIdAsync(int id);
    Task AddConversationAsync(Conversation conversation);
    Task<List<Conversation>> GetConversationsForUserAsync(int userId);
    Task<List<Message>> GetMessagesForConversationAsync(int conversationId);
}
