using BikeMarket.Models;
using Business.Interface;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace BikeMarket.Controllers;

public class UserRatingsController : Controller
{
    private readonly IUserRatingService _userRatingService;
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public UserRatingsController(IUserRatingService userRatingService, IOrderService orderService, IUserService userService)
    {
        _userRatingService = userRatingService;
        _orderService = orderService;
        _userService = userService;
    }

    public async Task<IActionResult> Create(int orderId)
    {
        var sessionUserId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return RedirectToAction("Login", "Users");
        }

        var raterId = int.Parse(sessionUserId);
        var order = await _orderService.GetByIdAsync(orderId);
        if (order == null)
        {
            return NotFound();
        }

        if (order.BuyerId != raterId || order.PaymentStatus != "paid")
        {
            return Forbid();
        }

        var existing = await _userRatingService.GetByOrderAndRaterAsync(order.Id, raterId);
        if (existing != null)
        {
            return RedirectToAction("Owner", "Users", new { id = order.SellerId });
        }

        var viewModel = new UserRatingCreateViewModel
        {
            OrderId = order.Id,
            RatedUserId = order.SellerId,
            SellerName = order.Seller?.Name ?? string.Empty,
            VehicleTitle = order.Vehicle?.Title ?? string.Empty
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserRatingCreateViewModel model)
    {
        var sessionUserId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return RedirectToAction("Login", "Users");
        }

        var raterId = int.Parse(sessionUserId);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var order = await _orderService.GetByIdAsync(model.OrderId);
        if (order == null)
        {
            return NotFound();
        }

        if (order.BuyerId != raterId || order.PaymentStatus != "paid")
        {
            return Forbid();
        }

        var existing = await _userRatingService.GetByOrderAndRaterAsync(order.Id, raterId);
        if (existing != null)
        {
            return RedirectToAction("Owner", "Users", new { id = order.SellerId });
        }

        var rating = new UserRating
        {
            OrderId = order.Id,
            RaterId = raterId,
            RatedUserId = order.SellerId,
            Rating = (byte)model.Rating,
            Comment = model.Comment,
            CreatedAt = DateTime.Now
        };

        await _userRatingService.CreateAsync(rating);

        var sellerRatings = await _userRatingService.GetByRatedUserAsync(order.SellerId);
        var seller = await _userService.GetByIdAsync(order.SellerId);
        if (seller != null)
        {
            seller.RatingAvg = sellerRatings.Any()
                ? (decimal)sellerRatings.Average(r => r.Rating)
                : 0m;
            await _userService.UpdateAsync(seller);
        }

        TempData["SuccessMessage"] = "?ánh giá ?ã ???c g?i!";
        return RedirectToAction("Owner", "Users", new { id = order.SellerId });
    }
}
