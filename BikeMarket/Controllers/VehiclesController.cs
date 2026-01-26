using BikeMarket.Controllers.Service;
using BikeMarket.Dtos;
using BikeMarket.Dtos.Vehicle;
using BikeMarket.Models;
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
        private readonly VehicleMarketContext _context;
        private readonly PhotoService _photoService;

        public VehiclesController(VehicleMarketContext context, PhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var vehicleMarketContext = _context.Vehicles.Include(v => v.Brand).Include(v => v.Category).Include(v => v.Seller);
            return View(await vehicleMarketContext.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            var userIdStr = HttpContext.Session.GetString("UserId");
            int? currentUserId = string.IsNullOrEmpty(userIdStr) ? null : int.Parse(userIdStr);

            // Fetch vehicle first
            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Include(v => v.Seller)
                .Include(v => v.VehicleImages)
                .FirstOrDefaultAsync(v => v.Id == id.Value);

            if (vehicle == null)
                return NotFound();

            bool isWishlisted = false;
            if (currentUserId != null)
            {
                isWishlisted = await _context.Wishlists
                    .AnyAsync(w => w.BuyerId == currentUserId && w.VehicleId == vehicle.Id);
            }


            var dto = new VehicleDetailDTO
            {
                VehicleId = vehicle.Id,
                Title = vehicle.Title,
                Description = vehicle.Description,

                BrandId = vehicle.BrandId,
                BrandName = vehicle.Brand?.Name ?? "Unknown",

                CategoryId = vehicle.CategoryId,
                CategoryName = vehicle.Category?.Name ?? "Unknown",

                Price = vehicle.Price,

                FrameSize = vehicle.FrameSize,
                Condition = vehicle.Condition,
                YearManufactured = vehicle.YearManufactured,
                Color = vehicle.Color,
                Location = vehicle.Location,
                Status = vehicle.Status,
                IsWishlisted = isWishlisted,

                SellerId = vehicle.SellerId,
                SellerName = vehicle.Seller?.Name ?? "Unknown",

                CreatedAt = vehicle.CreatedAt,

                ImageUrls = vehicle.VehicleImages
                    .Select(img => img.ImageUrl)
                    .ToList()
            };

            return View(dto);
        }


        // GET: Vehicles/Create
        public IActionResult Create()
        {
            Console.WriteLine("📋 [GET] Create() - Loading create form");
            
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id");

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
                
                ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", vehicle.CategoryId);
                return View(vehicle);
            }


            // ✅ Get UserId from Claims (Cookie Authentication)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var sellerId))
            {
                Console.WriteLine("❌ [POST] Create() - Invalid or missing user claim");
                ModelState.AddModelError("", "User not authenticated. Please log in again.");
                ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", vehicle.CategoryId);
                return View(vehicle);
            }

            vehicle.SellerId = sellerId;
            vehicle.CreatedAt = DateTime.Now;
            vehicle.Status = string.IsNullOrEmpty(vehicle.Status) ? "available" : vehicle.Status;

            Console.WriteLine($"📍 Vehicle prepared: SellerId={vehicle.SellerId}, Status={vehicle.Status}, CreatedAt={vehicle.CreatedAt}");

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            if (images != null && images.Count > 0)
            {                
                int imageCount = 0;
                foreach (var file in images)
                {
                    Console.WriteLine($"   📤 Uploading image {++imageCount}: {file.FileName} (Size: {file.Length} bytes)");
                    
                    var imageUrl = await _photoService.UploadImageAsync(file);
                    Console.WriteLine($"   ✅ Image uploaded: {imageUrl}");

                    var vehicleImage = new VehicleImage
                    {
                        VehicleId = vehicle.Id,
                        ImageUrl = imageUrl
                    };

                    _context.VehicleImages.Add(vehicleImage);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("⚠️ [POST] Create() - No images provided");
            }

            Console.WriteLine($"🎉 [POST] Create() - Vehicle created successfully! Redirecting to Index...");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.VehicleImages)
                .FirstOrDefaultAsync(v => v.Id == id.Value);

            if (vehicle == null) return NotFound();

            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", vehicle.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", vehicle.CategoryId);

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
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", vehicle.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", vehicle.CategoryId);
            ViewData["SellerId"] = new SelectList(_context.Users, "Id", "Id", vehicle.SellerId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Include(v => v.Seller)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
