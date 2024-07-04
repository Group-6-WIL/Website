// IUserService.cs

using System.Collections.Generic;
using Prog2.Models; // Import the namespace where User class is defined

namespace Prog2.Models
{
    // Interface defining methods for user service
    public interface IUserService
    {
        // Method to retrieve all users
        List<User> GetAllUsers();

        // Method to retrieve a user by ID
        User GetUserById(string id);

        // Method to delete a user by ID
        void DeleteUser(string id);
    }
}
