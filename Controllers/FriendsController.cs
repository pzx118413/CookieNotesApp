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
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public FriendsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Friends/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ActivePage"] = "Friends"; // Keep Friendslist highlighted or add "Friends"

            // Get confirmed friends (where I am Requester OR Receiver)
            var friends = await _context.Friends
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => (f.RequesterId == user.Id || f.ReceiverId == user.Id) 
                            && f.Status == FriendStatus.Accepted)
                .ToListAsync();

            // Get pending requests waiting for ME to accept
            var incomingRequests = await _context.Friends
                .Include(f => f.Requester)
                .Where(f => f.ReceiverId == user.Id && f.Status == FriendStatus.Pending)
                .ToListAsync();

            ViewBag.CurrentUserId = user.Id;
            ViewBag.Requests = incomingRequests;
            
            return View(friends);
        }

        // POST: /Friends/Add
        [HttpPost]
        public async Task<IActionResult> Add(string username)
        {
            var me = await _userManager.GetUserAsync(User);
            var targetUser = await _userManager.FindByNameAsync(username);

            if (targetUser == null)
            {
                TempData["Message"] = "User not found!";
                return RedirectToAction(nameof(Index));
            }

            if (targetUser.Id == me.Id)
            {
                TempData["Message"] = "You cannot add yourself!";
                return RedirectToAction(nameof(Index));
            }

            // Check if relationship already exists
            var existing = await _context.Friends
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == me.Id && f.ReceiverId == targetUser.Id) ||
                    (f.RequesterId == targetUser.Id && f.ReceiverId == me.Id));

            if (existing != null)
            {
                TempData["Message"] = "You are already friends or a request is pending.";
                return RedirectToAction(nameof(Index));
            }

            // Create Request
            var friendship = new Friend
            {
                RequesterId = me.Id,
                ReceiverId = targetUser.Id,
                Status = FriendStatus.Pending
            };

            _context.Friends.Add(friendship);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Friend request sent to {username}!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Friends/Accept
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var request = await _context.Friends.FindAsync(id);

            // Only accept if I am the receiver
            if (request != null && request.ReceiverId == user.Id)
            {
                request.Status = FriendStatus.Accepted;
                
                // Friend counts
                user.FriendsCount++; 
                var sender = await _userManager.FindByIdAsync(request.RequesterId);
                if(sender != null) sender.FriendsCount++;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        // POST: /Friends/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var friendship = await _context.Friends.FindAsync(id);

            // Security Check: Only delete if I am the Requester OR the Receiver
            if (friendship != null && (friendship.RequesterId == user.Id || friendship.ReceiverId == user.Id))
            {
                _context.Friends.Remove(friendship);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Friend removed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
    
}