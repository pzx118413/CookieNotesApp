using Microsoft.AspNetCore.Identity;

namespace CookieNotesApp.Models
{
    public class User : IdentityUser
    {
        public int CookiesReceived { get; set; } = 0;
        public string ProfileImageUrl { get; set; } = "/images/imagepfpholder.png";
        
        // This fixes the error in FriendsController
        public int FriendsCount { get; set; } = 0; 
    }
}