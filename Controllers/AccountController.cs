using Microsoft.AspNetCore.Mvc;

namespace CookieNotesApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
