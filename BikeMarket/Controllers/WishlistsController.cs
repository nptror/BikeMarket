using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using DTO.Vehicle;

namespace BikeMarket.Controllers
{
    public class WishlistsController : Controller
    {
        private readonly VehicleMarketContext _context;

        public WishlistsController(VehicleMarketContext context)
        {
            _context = context;
        }

        // GET: Wishlists
        public async Task<IActionResult> Index()
        {
            var vehicleMarketContext = _context.Wishlists.Include(w => w.Buyer).Include(w => w.Vehicle);
            return View(await vehicleMarketContext.ToListAsync());
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

            // ✅ Kiểm tra vehicle tồn tại
            var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId);
            System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Vehicle exists: {vehicleExists}");

            if (!vehicleExists)
            {
                System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Vehicle not found with ID: {request.VehicleId}");
                return Json(new { success = false, message = "VEHICLE_NOT_FOUND" });
            }

            var existed = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.BuyerId == userId && w.VehicleId == request.VehicleId);
            System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Existing wishlist found: {existed != null}");

            bool isWishlisted;

            if (existed != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Removing wishlist entry with ID: {existed.Id}");
                _context.Wishlists.Remove(existed);
                isWishlisted = false;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Adding new wishlist for UserId: {userId}, VehicleId: {request.VehicleId}");
                _context.Wishlists.Add(new Wishlist
                {
                    BuyerId = userId,
                    VehicleId = request.VehicleId,
                    CreatedAt = DateTime.Now
                });
                isWishlisted = true;
            }

            await _context.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine($"[ToggleAjax] Changes saved. IsWishlisted: {isWishlisted}");

            return Json(new { success = true, isWishlisted });
        }
        // GET: Wishlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.Buyer)
                .Include(w => w.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wishlist == null)
            {
                return NotFound();
            }

            return View(wishlist);
        }

        // GET: Wishlists/Create
        public IActionResult Create()
        {
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id");
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
                _context.Add(wishlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Id", wishlist.BuyerId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", wishlist.VehicleId);
            return View(wishlist);
        }

        // GET: Wishlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
            {
                return NotFound();
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Id", wishlist.BuyerId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", wishlist.VehicleId);
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
                    _context.Update(wishlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishlistExists(wishlist.Id))
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
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Id", wishlist.BuyerId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", wishlist.VehicleId);
            return View(wishlist);
        }

        // GET: Wishlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.Buyer)
                .Include(w => w.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WishlistExists(int id)
        {
            return _context.Wishlists.Any(e => e.Id == id);
        }
    }
}
