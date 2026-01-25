using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CookieNotesApp.Data;
using CookieNotesApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace CookieNotesApp.Controllers
{
    [Authorize(Roles = "Admin")] // <--- Only Admins allowed
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List All Users
        public async Task<IActionResult> Users()
        {
            ViewData["ActivePage"] = "Admin";
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // List All Notes
        public async Task<IActionResult> Notes()
        {
            ViewData["ActivePage"] = "Admin";
            var notes = await _context.Notes.Include(n => n.Author).OrderByDescending(n => n.CreatedAt).ToListAsync();
            return View(notes);
        }

        // Delete a User (and all their data)
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Prevent deleting yourself!
                if (User.Identity.Name == user.UserName) 
                {
                    TempData["Error"] = "You cannot delete your own admin account.";
                    return RedirectToAction(nameof(Users));
                }

                // Cleanup Data (Same logic as AccountController)
                var userNotes = _context.Notes.Where(n => n.UserId == user.Id);
                _context.Notes.RemoveRange(userNotes);

                var friends = _context.Friends.Where(f => f.RequesterId == user.Id || f.ReceiverId == user.Id);
                _context.Friends.RemoveRange(friends);

                var likes = _context.NoteLikes.Where(l => l.UserId == user.Id);
                _context.NoteLikes.RemoveRange(likes);

                await _context.SaveChangesAsync();
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Users));
        }

        // Delete a Note (Any note)
        [HttpPost]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Notes));
        }
    }
}