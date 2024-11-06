using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wil_v3.Models;

namespace Wil_v3.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Return the registration view
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed: {Errors}", ModelState.Values.SelectMany(v => v.Errors));
                return View(model); // Return to the registration view with validation errors
            }

            try
            {
                // Call the RegisterUser method to create a new user
                var userId = await RegisterUser(model.Email, model.Password);

                // Optionally store other user details (First Name, Surname, Role) in your database
                _logger.LogInformation("User registered successfully with ID: {UserId}", userId);

                return RedirectToAction("Login"); // Redirect to login or another action
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase authentication exceptions
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogError("FirebaseAuthException: {Message}", ex.Message);
                return View(model); // Return to the registration view with an error
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                _logger.LogError("Unexpected exception: {Message}", ex.Message);
                return View(model); // Return to the registration view with an error
            }
        }

        private async Task<string> RegisterUser(string email, string password)
        {
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                Email = email,
                Password = password
            });

            return userRecord.Uid; // Return the new user ID
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Return the login view
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View(); // Return to the login view with an error
            }

            try
            {
                // Call the LoginUser method to authenticate the user
                var token = await LoginUser(email, password);

                // Store token in session or cookies as needed
                HttpContext.Session.SetString("AuthToken", token); // Example using session storage

                _logger.LogInformation("User logged in successfully: {Email}", email);
                return RedirectToAction("Index", "Home"); // Redirect to a secure area of your application
            }
            catch (Exception ex)
            {
                // Handle authentication exceptions
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogError("Login failed for user {Email}: {Message}", email, ex.Message);
                return View(); // Return to the login view with an error
            }
        }

        private async Task<string> LoginUser(string email, string password)
        {
            var apiKey = "AIzaSyBhdSWhIagEjD4DaqA5JGpSAHwoK_oIVBY"; // Ensure this key is correct
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(url, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Firebase Login Successful. Response: {JsonResponse}", jsonResponse);
                    var tokenResponse = JsonSerializer.Deserialize<Google.Apis.Auth.OAuth2.Responses.TokenResponse>(jsonResponse);

                    if (tokenResponse?.IdToken != null)
                    {
                        return tokenResponse.IdToken;
                    }

                    throw new Exception("ID Token is missing from the response.");
                }
                else
                {
                    _logger.LogError("Login failed with status code {StatusCode}. Response: {JsonResponse}", response.StatusCode, jsonResponse);

                    // Attempt to extract Firebase-specific error details
                    var firebaseError = JsonSerializer.Deserialize<FirebaseError>(jsonResponse);
                    throw new Exception(firebaseError?.Error?.Message ?? "Login failed. Unknown error.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError("Network or HTTP error: {Message}", httpEx.Message);
                throw new Exception("Network issue occurred. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception during login: {Message}", ex.Message);
                throw new Exception("An error occurred during login. Please try again.");
            }
        }


        public class FirebaseError
        {
            public FirebaseErrorDetails Error { get; set; }
        }

        public class FirebaseErrorDetails
        {
            public string Message { get; set; }
            public string[] Errors { get; set; }
        }

    }
}
