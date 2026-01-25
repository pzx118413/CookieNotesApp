using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookieNotesApp.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }

        // The user who sent the friend request
        public string RequesterId { get; set; } = string.Empty;
        [ForeignKey("RequesterId")]
        public virtual User? Requester { get; set; }

        // The user receiving the request
        public string ReceiverId { get; set; } = string.Empty;
        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }

        // Status: 0 = Pending, 1 = Accepted
        public FriendStatus Status { get; set; } = FriendStatus.Pending;
    }

    public enum FriendStatus
    {
        Pending,
        Accepted
    }
}