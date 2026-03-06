using Business.Interface;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeMarket.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllWithVehiclesAsync();
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryService.GetByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            // Check for duplicate category name
            var existing = await _categoryService.GetAllAsync();
            if (existing.Any(c => string.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Name", "Category already exists.");
            }

            if (ModelState.IsValid)
            {
                await _categoryService.CreateAsync(category);
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryService.GetByIdWithVehiclesAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
                return NotFound();

            ModelState.Remove("Vehicles");

            // Check for duplicate category name (excluding current category)
            var existing = await _categoryService.GetAllAsync();
            if (existing.Any(c => c.Id != id && string.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Name", "Category already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.UpdateAsync(category);
                    TempData["SuccessMessage"] = "Category updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _categoryService.ExistsAsync(category.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryService.GetByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting category: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
