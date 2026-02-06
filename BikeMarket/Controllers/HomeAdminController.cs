using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeMarket.Controllers
{
    [Authorize(Roles = "admin")]
    public class HomeAdminController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeAdminController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _dashboardService.GetAdminDashboardStatsAsync();
            return View(stats);
        }
    }
}
