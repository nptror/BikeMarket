using Business.Interface;
using DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace BikeMarket.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
       
        public async Task<IActionResult> Index(string? search = null, decimal ratingAvg = 0,
            string? role = null, string? sortBy = "email", string? sortOrder = "asc")
        {
            var users = await _adminService.GetAllUserAsync(search, ratingAvg, role, sortBy, sortOrder);
            return View(users);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserProfileDTO model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var updateDto = new UpdateUserDto
                {
                    Phone = model.Phone,
                    Role = model.Role
                };

                await _adminService.UpdateUserAsync(id, updateDto);
                TempData["SuccessMessage"] = "Cập nhật user thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi cập nhật: {ex.Message}");
                return View(model);
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
    }
}
