using DTO.Vehicle;
using BikeMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BikeMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly VehicleMarketContext _context = new VehicleMarketContext();

        public HomeController(ILogger<HomeController> logger, VehicleMarketContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category)
                .Include(v => v.VehicleImages)
                .Where(v => v.Status == null || v.Status == "available") // ch? hi?n xe ?ang bán
                .OrderByDescending(v => v.CreatedAt)
                .Select(v => new VehicleListDTO
                {
                    VehicleId = v.Id,
                    Title = v.Title,

                    BrandName = v.Brand.Name,
                    CategoryName = v.Category.Name,

                    Price = v.Price,
                    FrameSize = v.FrameSize,
                    Condition = v.Condition,
                    Color = v.Color,
                    Location = v.Location,

                    Status = v.Status,
                    CreatedAt = v.CreatedAt,

                    ThumbnailUrl = v.VehicleImages
                        .OrderBy(img => img.Id)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return View(vehicles);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
