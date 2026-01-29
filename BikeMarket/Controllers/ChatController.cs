using BikeMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BikeMarket.Controllers
{
    public class ChatController : Controller
    {
        private readonly VehicleMarketContext _context;

        public ChatController(VehicleMarketContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? conversationId = null)
        {
            ViewData["ConversationId"] = conversationId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int vehicleId, int sellerId)
        {
            var buyerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerIdStr))
            {
                return RedirectToAction("Login", "Users");
            }

            var buyerId = int.Parse(buyerIdStr);

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.SellerId == sellerId && c.VehicleId == vehicleId);

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    BuyerId = buyerId,
                    SellerId = sellerId,
                    VehicleId = vehicleId,
                    CreatedAt = DateTime.Now,
                    BuyerUnreadCount = 0,
                    SellerUnreadCount = 0
                };

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { conversationId = conversation.Id });
        }
    }
}
