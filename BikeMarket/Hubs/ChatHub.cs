using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BikeMarket.Hubs
{
    public class ChatHub : Hub
    {
        private readonly VehicleMarketContext _context;

        public ChatHub(VehicleMarketContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            await base.OnConnectedAsync();
        }

        // ================= SEND MESSAGE =================
        public async Task SendMessage(int conversationId, string content)
        {
            var senderId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null) return;

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                Status = "sent",
                SentAt = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Update conversation
            conversation.LastMessageId = message.Id;
            conversation.LastMessageAt = message.SentAt;

            // xác định receiver
            int receiverId = senderId == conversation.BuyerId
                ? conversation.SellerId
                : conversation.BuyerId;

            if (senderId == conversation.BuyerId)
                conversation.SellerUnreadCount++;
            else
                conversation.BuyerUnreadCount++;

            await _context.SaveChangesAsync();

            // gửi realtime cho cả 2
            await Clients.Group($"user-{receiverId}")
                .SendAsync("ReceiveMessage",
                    conversationId, senderId, content);

            await Clients.Group($"user-{senderId}")
                .SendAsync("ReceiveMessage",
                    conversationId, senderId, content);
        }
    }
}
