using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Wil_v3.Models;

namespace Wil_v3.Controllers
{
    public class LocationsController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://ladybird-foundation-default-rtdb.firebaseio.com/"; // Use your Firebase URL here

        public LocationsController()
        {
            _httpClient = new HttpClient();
        }

        // GET: Locations/Index
        public async Task<IActionResult> Index()
        {
            var locations = await GetLocationsAsync();

            // Log the number of locations fetched
            Console.WriteLine($"Number of locations fetched: {locations.Count}");

            return View(locations);
        }


        // Fetch all locations from Firebase
        private async Task<List<Location>> GetLocationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/locations.json");
                response.EnsureSuccessStatusCode(); // Will throw if the status code is not 2xx

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw JSON from Firebase: {json}");

                // Deserialize the JSON data into a Dictionary first
                var locationsDict = JsonConvert.DeserializeObject<Dictionary<string, Location>>(json);

                // Check if the dictionary is null or empty
                if (locationsDict == null || !locationsDict.Any())
                {
                    Console.WriteLine("No locations found in Firebase.");
                    return new List<Location>();
                }

                // Return the list of locations while keeping the Firebase key
                return locationsDict.Select(l =>
                {
                    l.Value.LocationId = l.Key;  // Set the Firebase key as LocationId
                    return l.Value;
                }).ToList();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP error fetching locations: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON error deserializing locations: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching locations: {ex.Message}");
            }

            return new List<Location>();
        }

        // GET: Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocatioId, address,addressName")] Location location)
        {
            if (ModelState.IsValid)
            {
                await AddLocationAsync(location);
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // Add a new location to Firebase
        private async Task AddLocationAsync(Location location)
        {
            var json = JsonConvert.SerializeObject(location);
            var contentString = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/locations.json", contentString);
            response.EnsureSuccessStatusCode(); // Throws if not a success code.
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await GetLocationAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("LocationId,address,addressName")] Location location)
        {
            if (id != location.LocationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await UpdateLocationAsync(location);
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // Fetch a specific location from Firebase
        private async Task<Location> GetLocationAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/locations/{id}.json");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Location>(json);
            }
            return null;
        }

        // Update an existing location in Firebase
        private async Task UpdateLocationAsync(Location location)
        {
            var json = JsonConvert.SerializeObject(location);
            var contentString = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{BaseUrl}/locations/{location.LocationId}.json", contentString);
            response.EnsureSuccessStatusCode(); // Throws if not a success code.
        }
    }
}
