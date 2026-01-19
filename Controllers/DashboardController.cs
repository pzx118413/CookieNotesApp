using Microsoft.AspNetCore.Mvc;

namespace CookieNotesApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Dashboard";
            return View();
        }
    }
}
