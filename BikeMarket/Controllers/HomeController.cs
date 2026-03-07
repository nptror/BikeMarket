using DTO.Vehicle;
using BikeMarket.Models;
using Business.Interface;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BikeMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVehicleService _vehicleService;

        public HomeController(ILogger<HomeController> logger, IVehicleService vehicleService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _vehicleService.GetCategoriesAsync();
            var brands = await _vehicleService.GetBrandsAsync();
            var vehicles = await _vehicleService.GetAvailableListAsync();

            var viewModel = new HomeIndexViewModel
            {
                Categories = categories,
                Brands = brands,
                Vehicles = vehicles
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Search(string? keyword, int? categoryId, int? brandId)
        {
            var categories = await _vehicleService.GetCategoriesAsync();
            var brands = await _vehicleService.GetBrandsAsync();
            var vehicles = await _vehicleService.GetAvailableListAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                vehicles = vehicles
                    .Where(v => !string.IsNullOrEmpty(v.Title) && v.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (categoryId.HasValue)
            {
                var selectedCategory = categories.FirstOrDefault(c => c.Id == categoryId.Value);
                if (selectedCategory != null)
                {
                    vehicles = vehicles
                        .Where(v => string.Equals(v.CategoryName, selectedCategory.Name, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            if (brandId.HasValue)
            {
                vehicles = vehicles
                    .Where(v => v.BrandId == brandId.Value)
                    .ToList();
            }

            var viewModel = new HomeIndexViewModel
            {
                Categories = categories,
                Brands = brands,
                Keyword = keyword,
                CategoryId = categoryId,
                BrandId = brandId,
                Vehicles = vehicles
            };

            return View(viewModel);
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
