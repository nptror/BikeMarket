using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using DTO.Vehicle;

namespace BikeMarket.Controllers
{
    public class WishlistsController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistsController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // GET: Wishlists
        public async Task<IActionResult> Index()
        {
            return View(await _wishlistService.GetAllAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAjax([FromBody] WishlistRequest request)
        {
            System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Start - VehicleId: {request.VehicleId}");

            var userIdStr = HttpContext.Session.GetString("UserId");
            System.Diagnostics.Debug.WriteLine($"[ToggleAjax] UserIdStr from session: {userIdStr ?? "null"}");

            if (string.IsNullOrEmpty(userIdStr))
            {
                System.Diagnostics.Debug.WriteLine("[ToggleAjax] User not logged in");
                return Json(new { success = false, message = "NOT_LOGIN" });
            }

            // ✅ Validate request
            if (request?.VehicleId <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Invalid vehicleId: {request?.VehicleId}");
                return Json(new { success = false, message = "INVALID_VEHICLE_ID" });
            }

            int userId = int.Parse(userIdStr);
            System.Diagnostics.Debug.WriteLine($"[ToggleAjax] UserId parsed: {userId}");

            var result = await _wishlistService.ToggleAsync(userId, request.VehicleId);
            if (!result.Success)
            {
                System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Toggle failed: {result.ErrorMessage}");
                return Json(new { success = false, message = result.ErrorMessage ?? "ERROR" });
            }

            System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Changes saved. IsWishlisted: {result.IsWishlisted}");

            return Json(new { success = true, isWishlisted = result.IsWishlisted });
        }
        // GET: Wishlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _wishlistService.GetByIdAsync(id.Value);
            if (wishlist == null)
            {
                return NotFound();
            }

            return View(wishlist);
        }

        // GET: Wishlists/Create
        public async Task<IActionResult> Create()
        {
            ViewData["BuyerId"] = new SelectList(await _wishlistService.GetUsersAsync(), "Id", "Id");
            ViewData["VehicleId"] = new SelectList(await _wishlistService.GetVehiclesAsync(), "Id", "Id");
            return View();
        }

        // POST: Wishlists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BuyerId,VehicleId,CreatedAt")] Wishlist wishlist)
        {
            if (ModelState.IsValid)
            {
                await _wishlistService.CreateAsync(wishlist);
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(await _wishlistService.GetUsersAsync(), "Id", "Id", wishlist.BuyerId);
            ViewData["VehicleId"] = new SelectList(await _wishlistService.GetVehiclesAsync(), "Id", "Id", wishlist.VehicleId);
            return View(wishlist);
        }

        // GET: Wishlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _wishlistService.GetByIdAsync(id.Value);
            if (wishlist == null)
            {
                return NotFound();
            }
            ViewData["BuyerId"] = new SelectList(await _wishlistService.GetUsersAsync(), "Id", "Id", wishlist.BuyerId);
            ViewData["VehicleId"] = new SelectList(await _wishlistService.GetVehiclesAsync(), "Id", "Id", wishlist.VehicleId);
            return View(wishlist);
        }

        // POST: Wishlists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BuyerId,VehicleId,CreatedAt")] Wishlist wishlist)
        {
            if (id != wishlist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _wishlistService.UpdateAsync(wishlist);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await WishlistExists(wishlist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(await _wishlistService.GetUsersAsync(), "Id", "Id", wishlist.BuyerId);
            ViewData["VehicleId"] = new SelectList(await _wishlistService.GetVehiclesAsync(), "Id", "Id", wishlist.VehicleId);
            return View(wishlist);
        }

        // GET: Wishlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _wishlistService.GetByIdAsync(id.Value);
            if (wishlist == null)
            {
                return NotFound();
            }

            return View(wishlist);
        }

        // POST: Wishlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _wishlistService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> WishlistExists(int id)
        {
            return _wishlistService.ExistsAsync(id);
        }
    }
}
