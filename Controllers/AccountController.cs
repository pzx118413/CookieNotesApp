using Microsoft.AspNetCore.Mvc;

namespace CookieNotesApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Profile()
        {
            ViewData["ActivePage"] = "Account";
            return View();

        }
            public IActionResult Index() => View();
            public IActionResult Login() => View();
            public IActionResult Register() => View();

    }
}
