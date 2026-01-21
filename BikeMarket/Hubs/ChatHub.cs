using BikeMarket.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BikeMarket.Hubs
{
    public class ChatHub : Hub
    {
        private readonly VehicleMarketContext _context;

        // Map userId -> connectionId (demo level)
        private static Dictionary<int, string> UserConnections = new();

        public ChatHub(VehicleMarketContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            UserConnections[userId] = Context.ConnectionId;

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            UserConnections.Remove(userId);

            await base.OnDisconnectedAsync(exception);
        }

        // ================= SEND MESSAGE =================
        public async Task SendMessage(int conversationId, string message)
        {
            var senderId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null) return;

            // Lưu message DB
            var msg = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = message,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(msg);

            // Update last message
            conversation.LastMessageAt = DateTime.Now;
            conversation.LastMessageText = message;

            await _context.SaveChangesAsync();

            // Xác định người nhận
            int receiverId = senderId == conversation.BuyerId
                ? conversation.SellerId
                : conversation.BuyerId;

            // Gửi realtime
            if (UserConnections.ContainsKey(receiverId))
            {
                await Clients.Client(UserConnections[receiverId])
                    .SendAsync("ReceiveMessage", senderId, message);
            }

            // Echo lại cho người gửi
            await Clients.Caller.SendAsync("ReceiveMessage", senderId, message);
        }
    }
}
