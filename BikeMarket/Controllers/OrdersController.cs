using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interface;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BikeMarket.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
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

            var order = await _orderService.BuyNowAsync(buyerId, vehicleId);
            if (order == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id)
        {
            if (!await _orderService.PayAsync(id))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _orderService.GetAllAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetByIdAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            ViewData["BuyerId"] = new SelectList(await _orderService.GetUsersAsync(), "Id", "Id");
            ViewData["SellerId"] = new SelectList(await _orderService.GetUsersAsync(), "Id", "Id");
            ViewData["VehicleId"] = new SelectList(await _orderService.GetVehiclesAsync(), "Id", "Id");
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
                await _orderService.CreateAsync(order);
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(await _orderService.GetUsersAsync(), "Id", "Id", order.BuyerId);
            ViewData["SellerId"] = new SelectList(await _orderService.GetUsersAsync(), "Id", "Id", order.SellerId);
            ViewData["VehicleId"] = new SelectList(await _orderService.GetVehiclesAsync(), "Id", "Id", order.VehicleId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetByIdAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["BuyerId"] = new SelectList(await _orderService.GetUsersAsync(), "Id", "Id", order.BuyerId);
            ViewData["SellerId"] = new SelectList(await _orderService.GetUsersAsync(), "Id", "Id", order.SellerId);
            ViewData["VehicleId"] = new SelectList(await _orderService.GetVehiclesAsync(), "Id", "Id", order.VehicleId);
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
                    await _orderService.UpdateAsync(order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OrderExists(order.Id))
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
            ViewData["BuyerId"] = new SelectList(await _orderService.GetUsersAsync(), "Id", "Id", order.BuyerId);
            ViewData["SellerId"] = new SelectList(await _orderService.GetUsersAsync(), "Id", "Id", order.SellerId);
            ViewData["VehicleId"] = new SelectList(await _orderService.GetVehiclesAsync(), "Id", "Id", order.VehicleId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetByIdAsync(id.Value);
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
            await _orderService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> OrderExists(int id)
        {
            return _orderService.ExistsAsync(id);
        }
    }
}
