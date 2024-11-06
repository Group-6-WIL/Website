using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wil_v3.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Wil_v3.Controllers
{
    public class EventsController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://ladybird-foundation-default-rtdb.firebaseio.com/"; // Updated to correct Base URL

        public EventsController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index(DateTime? searchDate)
        {
            var events = await GetEventsAsync();

            // Log the number of events fetched
            Console.WriteLine($"Number of events fetched: {events.Count}");

            // Apply date filtering if searchDate is provided
            if (searchDate.HasValue)
            {
                events = events.Where(e => e.date.Date == searchDate.Value.Date).ToList();
                Console.WriteLine($"Filtered events count after date search: {events.Count}");
            }


            // Order events by date
            events = events.OrderBy(e => e.date).ToList();

            return View(events);
        }


        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("eventName,date,description,imageUrl")] Events events)
        {
            if (ModelState.IsValid)
            {
                await AddEventAsync(events);
                return RedirectToAction(nameof(Index));
            }
            return View(events);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await GetEventAsync(id.Value);
            if (events == null)
            {
                return NotFound();
            }
            return View(events);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("eventId,eventName,date,description,imageUrl")] Events events)
        {
            if (id != events.eventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await UpdateEventAsync(events);
                return RedirectToAction(nameof(Index));
            }
            return View(events);
        }




        private async Task<List<Events>> GetEventsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/events.json");
                response.EnsureSuccessStatusCode(); // Will throw if the status code is not 2xx

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw JSON from Firebase: {json}");

                // Deserialize into a Dictionary first to get Firebase keys
                var eventsDict = JsonConvert.DeserializeObject<Dictionary<string, Events>>(json);

                // Check if eventsDict is null or empty
                if (eventsDict == null || !eventsDict.Any())
                {
                    Console.WriteLine("No events found in Firebase.");
                    return new List<Events>();
                }

                return eventsDict.Select(e =>
                {
                    e.Value.eventId = e.Key;  // Keep eventId as a string
                    return e.Value;
                }).ToList();

            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP error fetching events: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON error deserializing events: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching events: {ex.Message}");
            }

            return new List<Events>();
        }


        // Fetch a specific event from Firebase
        private async Task<Events> GetEventAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/events/{id}.json");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Events>(json);
            }
            return null;
        }


        // Add a new event to Firebase
        private async Task AddEventAsync(Events events)
        {
            var json = JsonConvert.SerializeObject(events);
            var contentString = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/events.json", contentString);
            response.EnsureSuccessStatusCode(); // Throws if not a success code.
        }

        // Update an existing event in Firebase
        private async Task UpdateEventAsync(Events events)
        {
            var json = JsonConvert.SerializeObject(events);
            var contentString = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{BaseUrl}/events/{events.eventId}.json", contentString);
            response.EnsureSuccessStatusCode(); // Throws if not a success code.
        }
    }
}
