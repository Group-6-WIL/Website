using System;
using System.Linq;

using WIL_v2_test.Data;

namespace Prog2.Models
{
    // Service class responsible for user-related operations
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        // Constructor injection to get an instance of ApplicationDbContext
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to retrieve all users from the database
        public List<User> GetAllUsers()
        {
            // Retrieve all users from the database
            var users = _context.Users.ToList();

            // Convert ApplicationUser objects to User model objects
            var userList = users.Select(u => new User
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email
            }).ToList();

            return userList;
        }

        // Method to retrieve a user by ID
        public User GetUserById(string id)
        {
            // Retrieve a user by ID from the database
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Convert ApplicationUser to User model (if necessary)
            var userModel = new User
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email
            };

            return userModel;
        }

        // Method to delete a user by ID
        public void DeleteUser(string id)
        {
            // Retrieve the user from the database
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                _context.Users.Remove(user); // Remove the user
                _context.SaveChanges(); // Save changes to the database
            }
            else
            {
                throw new InvalidOperationException("User not found");
            }
        }
    }
}
