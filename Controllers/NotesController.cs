using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CookieNotesApp.Data;
using CookieNotesApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CookieNotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public NotesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ActivePage"] = "Notes";

            var myNotes = await _context.Notes
                .Where(n => n.UserId == user.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(myNotes);
        }

        public async Task<IActionResult> Editor(int? id)
        {
            if (id == null) return View(new Note());

            var user = await _userManager.GetUserAsync(User);
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == user.Id);

            if (note == null) return NotFound();

            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> Save(Note note)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (string.IsNullOrWhiteSpace(note.Title)) note.Title = "Unnamed note";

            if (note.Id == 0)
            {
                // Create New
                note.UserId = user.Id;
                note.CreatedAt = DateTime.Now;
                // note.Visibility is mapped automatically from the dropdown
                _context.Notes.Add(note);
            }
            else
            {
                // Update Existing
                var existingNote = await _context.Notes
                    .FirstOrDefaultAsync(n => n.Id == note.Id && n.UserId == user.Id);
                
                if (existingNote == null) return NotFound();

                existingNote.Title = note.Title;
                existingNote.Content = note.Content;
                existingNote.Tags = note.Tags;
                existingNote.Visibility = note.Visibility;
                
                _context.Update(existingNote);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Notes/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == user.Id);

            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}