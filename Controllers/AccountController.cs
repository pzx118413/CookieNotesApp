using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CookieNotesApp.Data;
using CookieNotesApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace CookieNotesApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewData["ActivePage"] = "Account"; 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewData["ActivePage"] = "Account";
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);
                
                if (result.Succeeded) 
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewData["ActivePage"] = "Account";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string username)
        {
            ViewData["ActivePage"] = "Account";
            if (ModelState.IsValid)
            {
                var user = new User { UserName = username, Email = email, ProfileImageUrl = "/images/imagepfpholder.png" };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Dashboard");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            ViewData["ActivePage"] = "Account";
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
            return View(user);
        }

        // POST: /Account/DeleteAccount
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Delete User's Notes
            var myNotes = _context.Notes.Where(n => n.UserId == user.Id);
            _context.Notes.RemoveRange(myNotes);

            // Delete Friendships
            var myFriends = _context.Friends.Where(f => f.RequesterId == user.Id || f.ReceiverId == user.Id);
            _context.Friends.RemoveRange(myFriends);

            // Delete Likes
            var myLikes = _context.NoteLikes.Where(l => l.UserId == user.Id);
            _context.NoteLikes.RemoveRange(myLikes);

            // Save cleanup
            await _context.SaveChangesAsync();

            // Delete User & Sign Out
            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index", "Home");
        }
    }
}