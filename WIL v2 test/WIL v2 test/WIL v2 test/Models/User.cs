using Microsoft.AspNetCore.Identity;

namespace Prog2.Models
{
    // Model class representing a user, extending IdentityUser provided by ASP.NET Core Identity
    public class User : IdentityUser
    {
        // Property for the user's ID
        public string Id { get; set; }

        // Property for the user's name
        public string Name { get; set; }

        // Property for the user's email
        public string Email { get; set; }

        // Property for the user's roles
        public string Roles { get; set; }
    }
}
