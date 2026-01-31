using DataAccess.Models;

namespace Business.Interface;

public interface IChatService
{
    Task<int> GetOrCreateConversationIdAsync(int buyerId, int sellerId);
    Task<List<Conversation>> GetConversationsAsync(int userId);
    Task<List<Message>> GetMessagesAsync(int conversationId);
}
