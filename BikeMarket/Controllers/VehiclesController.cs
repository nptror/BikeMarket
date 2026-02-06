using Business.Interface;
using DataAccess.Models;
using DTO.Vehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BikeMarket.Models;

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

        // GET: Vehicles/MyPost
        public async Task<IActionResult> MyPost(string? tab)
        {
            var sellerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sellerIdStr) || !int.TryParse(sellerIdStr, out var sellerId))
            {
                return RedirectToAction("Login", "Users");
            }

            var summary = await _vehicleService.GetMyPostSummaryAsync(sellerId);
            var activeTab = string.IsNullOrWhiteSpace(tab) ? "display" : tab.Trim().ToLowerInvariant();

            IEnumerable<VehicleListDTO> filteredVehicles = summary.Vehicles;
            if (activeTab == "draft")
            {
                filteredVehicles = summary.Vehicles
                    .Where(v => !string.IsNullOrEmpty(v.Status) && v.Status.Equals("draft", StringComparison.OrdinalIgnoreCase));
            }
            else if (activeTab == "pending")
            {
                filteredVehicles = summary.Vehicles
                    .Where(v => !string.IsNullOrEmpty(v.Status) && v.Status.Equals("pending", StringComparison.OrdinalIgnoreCase));
            }
            else if (activeTab == "denied")
            {
                filteredVehicles = summary.Vehicles
                    .Where(v => !string.IsNullOrEmpty(v.Status) && v.Status.Equals("denied", StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                filteredVehicles = summary.Vehicles
                    .Where(v => string.IsNullOrEmpty(v.Status) || v.Status.Equals("available", StringComparison.OrdinalIgnoreCase));
                activeTab = "display";
            }

            var viewModel = new VehicleMyPostViewModel
            {
                DisplayCount = summary.DisplayCount,
                DraftCount = summary.DraftCount,
                PendingCount = summary.PendingCount,
                DeniedCount = summary.DeniedCount,
                ActiveTab = activeTab,
                Vehicles = filteredVehicles.ToList()
            };

            return View(viewModel);
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

            // Debug: Log số lượng ảnh nhận được
            Console.WriteLine($"📸 [POST] Create() - Received {images?.Count ?? 0} images");
            if (images != null)
            {
                foreach (var img in images)
                {
                    Console.WriteLine($"   - Image: {img.FileName} ({img.Length} bytes)");
                }
            }

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
            return RedirectToAction(nameof(MyPost));
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,SellerId,BrandId,CategoryId,Title,Description,Price,FrameSize,Condition,YearManufactured,Location,Status,Color,CreatedAt")] Vehicle vehicle, List<IFormFile>? newImages)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            // Remove navigation properties from ModelState
            ModelState.Remove("Brand");
            ModelState.Remove("Category");
            ModelState.Remove("Seller");
            ModelState.Remove("VehicleImages");

            // Debug: Log số lượng ảnh nhận được
            Console.WriteLine($"📸 [POST] Edit() - Received {newImages?.Count ?? 0} new images");
            if (newImages != null)
            {
                foreach (var img in newImages)
                {
                    Console.WriteLine($"   - Image: {img.FileName} ({img.Length} bytes)");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _vehicleService.UpdateAsync(vehicle);
                    
                    if (newImages != null && newImages.Count > 0)
                    {
                        Console.WriteLine($"📤 [POST] Edit() - Uploading {newImages.Count} images...");
                        await _vehicleService.AddImagesAsync(vehicle.Id, newImages);
                        Console.WriteLine($"✅ [POST] Edit() - Images uploaded successfully");
                    }
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
                return RedirectToAction(nameof(DetailsAdmin), new { id = vehicle.Id });
            }
            
            ViewData["BrandId"] = new SelectList(await _vehicleService.GetBrandsAsync(), "Id", "Name", vehicle.BrandId);
            ViewData["CategoryId"] = new SelectList(await _vehicleService.GetCategoriesAsync(), "Id", "Name", vehicle.CategoryId);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int vehicleId)
        {
            await _vehicleService.DeleteImageAsync(imageId);
            return RedirectToAction(nameof(Edit), new { id = vehicleId });
        }

        private Task<bool> VehicleExists(int id)
        {
            return _vehicleService.ExistsAsync(id);
        }
    }
}
