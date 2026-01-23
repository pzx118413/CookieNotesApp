using Microsoft.AspNetCore.Mvc;

namespace CookieNotesApp.Controllers
{
    public class NotesController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Notes";
            return View();
        }
        public IActionResult Editor()
        {
            return View();
        }
    }
}
