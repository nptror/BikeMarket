using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interface;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BikeMarket.Controllers
{
    [Authorize(Roles = "admin")]
    public class BrandsController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly IPhotoService _photoService;

        public BrandsController(IBrandService brandService, IPhotoService photoService)
        {
            _brandService = brandService;
            _photoService = photoService;
        }

        // GET: Brands
        public async Task<IActionResult> Index()
        {
            return View(await _brandService.GetAllWithVehiclesAsync());
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _brandService.GetByIdAsync(id.Value);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Brand brand, IFormFile? image)
        {
            ModelState.Remove("ImageUrl");

            if (image == null || image.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Please select an image for the brand.");
            }

            var existing = await _brandService.GetAllAsync();
            if (existing.Any(b => string.Equals(b.Name, brand.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Name", "Brand already exists.");
            }

            if (ModelState.IsValid)
            {
                brand.ImageUrl = await _photoService.UploadImageAsync(image!);
                await _brandService.CreateAsync(brand);
                TempData["SuccessMessage"] = "Brand created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _brandService.GetByIdWithVehiclesAsync(id.Value);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl")] Brand brand, IFormFile? image)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Vehicles");

            // Check for duplicate brand name (excluding current brand)
            var existing = await _brandService.GetAllAsync();
            if (existing.Any(b => b.Id != id && string.Equals(b.Name, brand.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Name", "Brand already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null && image.Length > 0)
                    {
                        brand.ImageUrl = await _photoService.UploadImageAsync(image);
                    }

                    await _brandService.UpdateAsync(brand);
                    TempData["SuccessMessage"] = "Brand updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BrandExists(brand.Id))
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
            return View(brand);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _brandService.GetByIdAsync(id.Value);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _brandService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Brand deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting brand: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> BrandExists(int id)
        {
            return _brandService.ExistsAsync(id);
        }
    }
}
