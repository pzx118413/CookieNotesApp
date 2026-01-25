using Microsoft.AspNetCore.Mvc;

namespace CookieNotesApp.Controllers
{
    public class FriendsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }
    }
}
