using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wil_v3.Models;

namespace Wil_v3.Controllers
{
 
    public class DonationController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://LadyBird_Foundation.firebaseio.com/"; // Replace with your Firebase database URL

        public DonationController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            var donations = await GetDonationsAsync();
            return View(donations);
        }

      

        private async Task<List<Donation>> GetDonationsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/donations.json");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Donation>>(json);
            }
            return new List<Donation>();
        }

        private async Task AddDonationAsync(Donation donation)
        {
            var json = JsonConvert.SerializeObject(donation);
            var contentString = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"{BaseUrl}/donations.json", contentString);
        }
    }
}
