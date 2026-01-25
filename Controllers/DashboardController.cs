using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CookieNotesApp.Data;
using CookieNotesApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CookieNotesApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ActivePage"] = "Dashboard"; // Needed for Navbar

            // Get Friend IDs
            var friendIds = await _context.Friends
                .Where(f => f.Status == FriendStatus.Accepted && 
                           (f.RequesterId == user.Id || f.ReceiverId == user.Id))
                .Select(f => f.RequesterId == user.Id ? f.ReceiverId : f.RequesterId)
                .ToListAsync();

            // Get Notes (Public + Friends Only)
            var feedNotes = await _context.Notes
                .Include(n => n.Author)
                .Where(n => n.UserId != user.Id && 
                           (n.Visibility == NoteVisibility.Public || 
                           (n.Visibility == NoteVisibility.FriendsOnly && friendIds.Contains(n.UserId))))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            // Get list of notes I have already liked
            var myLikes = await _context.NoteLikes
                .Where(nl => nl.UserId == user.Id)
                .Select(nl => nl.NoteId)
                .ToListAsync();

            // Pass the list of liked IDs to the view
            ViewBag.LikedNoteIds = myLikes;

            return View(feedNotes);
        }

        [HttpPost]
        public async Task<IActionResult> GiveCookie(int noteId)
        {
            var user = await _userManager.GetUserAsync(User);
            
            // CHECK: Has this user already liked this note?
            var alreadyLiked = await _context.NoteLikes
                .AnyAsync(nl => nl.NoteId == noteId && nl.UserId == user.Id);

            if (alreadyLiked)
            {
                // Stop! Don't give another cookie.
                return RedirectToAction(nameof(Index));
            }

            var note = await _context.Notes.Include(n => n.Author).FirstOrDefaultAsync(n => n.Id == noteId);
            if (note != null)
            {
                // Add to Note count
                note.CookieCount++;

                // Add to Author count
                if (note.Author != null) note.Author.CookiesReceived++;

                // RECORD the transaction
                _context.NoteLikes.Add(new NoteLike { NoteId = noteId, UserId = user.Id });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}