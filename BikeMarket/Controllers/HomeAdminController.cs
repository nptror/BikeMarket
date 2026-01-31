using Microsoft.AspNetCore.Mvc;

namespace BikeMarket.Controllers
{
    public class HomeAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
