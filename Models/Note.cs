using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookieNotesApp.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Tags { get; set; } = string.Empty;

        public int CookieCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // --- NEW PRIVACY SETTING ---
        // 0 = Private, 1 = FriendsOnly, 2 = Public
        public NoteVisibility Visibility { get; set; } = NoteVisibility.Private;

        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public virtual User? Author { get; set; }
    }

    public enum NoteVisibility
    {
        Private = 0,
        FriendsOnly = 1,
        Public = 2
    }
}