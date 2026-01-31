using Business.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BikeMarket.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public IActionResult Index(int? conversationId = null)
        {
            ViewData["ConversationId"] = conversationId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int sellerId)
        {
            var buyerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerIdStr))
            {
                return RedirectToAction("Login", "Users");
            }

            var buyerId = int.Parse(buyerIdStr);

            var conversationId = await _chatService.GetOrCreateConversationIdAsync(buyerId, sellerId);
            return RedirectToAction(nameof(Index), new { conversationId });
        }
    }
}
