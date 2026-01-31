using Business.Interface;
using DataAccess.Interface;
using DataAccess.Models;

namespace Business.Service;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<int> GetOrCreateConversationIdAsync(int buyerId, int sellerId)
    {
        var conversation = await _chatRepository.GetConversationAsync(buyerId, sellerId);
        if (conversation == null)
        {
            conversation = new Conversation
            {
                BuyerId = buyerId,
                SellerId = sellerId,
                CreatedAt = DateTime.Now,
                BuyerUnreadCount = 0,
                SellerUnreadCount = 0
            };

            await _chatRepository.AddConversationAsync(conversation);
        }

        return conversation.Id;
    }

    public Task<List<Conversation>> GetConversationsAsync(int userId)
    {
        return _chatRepository.GetConversationsForUserAsync(userId);
    }

    public Task<List<Message>> GetMessagesAsync(int conversationId)
    {
        return _chatRepository.GetMessagesForConversationAsync(conversationId);
    }
}
