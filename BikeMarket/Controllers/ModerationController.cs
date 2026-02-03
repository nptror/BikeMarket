using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeMarket.Controllers
{
    [Authorize(Roles = "admin")]
    public class ModerationController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public ModerationController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // GET: Moderation
        public async Task<IActionResult> Index()
        {
            var pendingVehicles = await _vehicleService.GetPendingVehiclesAsync();
            return View("~/Views/HomeAdmin/Moderation/Index.cshtml", pendingVehicles);
        }

        // GET: Moderation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var vehicle = await _vehicleService.GetVehicleForModerationAsync(id.Value);
            if (vehicle == null)
                return NotFound();

            return View("~/Views/HomeAdmin/Moderation/Details.cshtml", vehicle);
        }

        // POST: Moderation/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _vehicleService.ApproveVehicleAsync(id);
                TempData["SuccessMessage"] = "Post has been approved successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving post: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Moderation/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? reason)
        {
            try
            {
                await _vehicleService.RejectVehicleAsync(id, reason);
                TempData["SuccessMessage"] = "Post has been rejected!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting post: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
