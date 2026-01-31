using Business.Interface;
using DataAccess.Models;
using DTO.Vehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeMarket.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            return View(await _vehicleService.GetAllAsync());
        }

        // GET: Vehicles/DetailsAdmin/5
        public async Task<IActionResult> DetailsAdmin(int? id)
        {
            if (id == null)
                return NotFound();

            var dto = await _vehicleService.GetDetailAdminAsync(id.Value);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // GET: Vehicles/DetailsBuyer/5
        public async Task<IActionResult> DetailsBuyer(int? id)
        {
            if (id == null)
                return NotFound();

            var userIdStr = HttpContext.Session.GetString("UserId");
            int? currentUserId = string.IsNullOrEmpty(userIdStr) ? null : int.Parse(userIdStr);

            var dto = await _vehicleService.GetDetailBuyerAsync(id.Value, currentUserId);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // GET: Vehicles/Create
        public async Task<IActionResult> Create()
        {
            Console.WriteLine("📋 [GET] Create() - Loading create form");

            ViewData["BrandId"] = new SelectList(await _vehicleService.GetBrandsAsync(), "Id", "Name");
            ViewData["CategoryId"] = new SelectList(await _vehicleService.GetCategoriesAsync(), "Id", "Name");
            ViewData["SellerId"] = new SelectList(await _vehicleService.GetSellersAsync(), "Id", "Id");

            Console.WriteLine("✅ [GET] Create() - Form loaded successfully");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("BrandId,CategoryId,Title,Description,Price,FrameSize,Condition,YearManufactured,Location,Status,Color")]
            Vehicle vehicle,
            List<IFormFile>? images)
        {
            // ✅ Remove non-bindable navigation properties from ModelState
            ModelState.Remove("Brand");
            ModelState.Remove("Category");
            ModelState.Remove("Seller");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ [POST] Create() - ModelState Invalid");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"   Error: {error.ErrorMessage}");
                    }
                }

                ViewData["BrandId"] = new SelectList(await _vehicleService.GetBrandsAsync(), "Id", "Name", vehicle.BrandId);
                ViewData["CategoryId"] = new SelectList(await _vehicleService.GetCategoriesAsync(), "Id", "Name", vehicle.CategoryId);
                return View(vehicle);
            }


            // ✅ Get UserId from Claims (Cookie Authentication)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var sellerId))
            {
                Console.WriteLine("❌ [POST] Create() - Invalid or missing user claim");
                ModelState.AddModelError("", "User not authenticated. Please log in again.");
                ViewData["BrandId"] = new SelectList(await _vehicleService.GetBrandsAsync(), "Id", "Name", vehicle.BrandId);
                ViewData["CategoryId"] = new SelectList(await _vehicleService.GetCategoriesAsync(), "Id", "Name", vehicle.CategoryId);
                return View(vehicle);
            }

            Console.WriteLine($"📍 Vehicle prepared: SellerId={sellerId}");

            await _vehicleService.CreateAsync(vehicle, images, sellerId);

            Console.WriteLine($"🎉 [POST] Create() - Vehicle created successfully! Redirecting to Index...");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _vehicleService.GetForEditAsync(id.Value);
            if (vehicle == null) return NotFound();

            ViewData["BrandId"] = new SelectList(await _vehicleService.GetBrandsAsync(), "Id", "Name", vehicle.BrandId);
            ViewData["CategoryId"] = new SelectList(await _vehicleService.GetCategoriesAsync(), "Id", "Name", vehicle.CategoryId);

            return View(vehicle);
        }


        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SellerId,BrandId,CategoryId,Title,Description,Price,FrameSize,Condition,YearManufactured,Location,Status,Color,CreatedAt")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _vehicleService.UpdateAsync(vehicle);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await VehicleExists(vehicle.Id))
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
            ViewData["BrandId"] = new SelectList(await _vehicleService.GetBrandsAsync(), "Id", "Id", vehicle.BrandId);
            ViewData["CategoryId"] = new SelectList(await _vehicleService.GetCategoriesAsync(), "Id", "Id", vehicle.CategoryId);
            ViewData["SellerId"] = new SelectList(await _vehicleService.GetSellersAsync(), "Id", "Id", vehicle.SellerId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleService.GetForDeleteAsync(id.Value);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vehicleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> VehicleExists(int id)
        {
            return _vehicleService.ExistsAsync(id);
        }
    }
}
