// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace Wil_v3.Models
{
    public class User
    {
        public string Id { get; set; } // This can be the UID from Firebase
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }
    }
}
