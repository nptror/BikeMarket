using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BikeMarket.Models;
using System.Security.Claims;

namespace BikeMarket.Controllers
{
    public class OrdersController : Controller
    {
        private readonly VehicleMarketContext _context;

        public OrdersController(VehicleMarketContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyNow(int vehicleId)
        {
            var buyerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerIdStr))
            {
                return RedirectToAction("Login", "Users");
            }

            var buyerId = int.Parse(buyerIdStr);

            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == vehicleId);

            if (vehicle == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                BuyerId = buyerId,
                SellerId = vehicle.SellerId,
                VehicleId = vehicle.Id,
                TotalAmount = vehicle.Price,
                Status = "pending",
                PaymentStatus = "unpaid",
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            order.PaymentStatus = "paid";
            order.Status = "paid";
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var vehicleMarketContext = _context.Orders.Include(o => o.Buyer).Include(o => o.Seller).Include(o => o.Vehicle);
            return View(await vehicleMarketContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Include(o => o.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BuyerId,SellerId,VehicleId,Status,TotalAmount,PaymentMethod,PaymentStatus,Notes,CreatedAt,UpdatedAt")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Id", order.BuyerId);
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id", order.SellerId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", order.VehicleId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Id", order.BuyerId);
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id", order.SellerId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", order.VehicleId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BuyerId,SellerId,VehicleId,Status,TotalAmount,PaymentMethod,PaymentStatus,Notes,CreatedAt,UpdatedAt")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["BuyerId"] = new SelectList(_context.Users, "Id", "Id", order.BuyerId);
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id", order.SellerId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", order.VehicleId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Include(o => o.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
