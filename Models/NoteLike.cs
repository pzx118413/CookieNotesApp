using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookieNotesApp.Models
{
    public class NoteLike
    {
        [Key]
        public int Id { get; set; }

        public int NoteId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}