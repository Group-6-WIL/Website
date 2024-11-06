using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Wil_v3.Models;

namespace Wil_v3.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://ladybird-foundation-default-rtdb.firebaseio.com/"; // Replace with your Firebase database URL

        public AboutUsController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            var aboutUs = await GetAboutUsAsync();
            var model = new DashboardViewModel
            {
                AboutUsContent = aboutUs?.Content ?? "Default About Us content",
                MissionContent = aboutUs?.Mission ?? "Default Mission content",
                ImagePath = aboutUs?.ImagePath,
                LastUpdated = DateTime.Now // Set the current time
            };
            return View(model);
        }

        // Get About Us content from Firebase
        private async Task<AboutUs> GetAboutUsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/aboutUs.json");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Directly deserialize the AboutUs object instead of a dictionary
                var aboutUs = JsonConvert.DeserializeObject<AboutUs>(json);

                return aboutUs; // Return the AboutUs object
            }

            // Log an error if the request fails
            return null;
        }
    }
}
