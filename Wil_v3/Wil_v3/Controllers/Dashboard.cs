using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wil_v3.Models;

namespace Wil_v3.Controllers
{
    //[Authorize(Policy = "RequireAdminRole")]
    public class DashboardController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<DashboardController> _logger;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://ladybird-foundation-default-rtdb.firebaseio.com/"; // Replace with your database URL

        public DashboardController(IWebHostEnvironment hostingEnvironment, ILogger<DashboardController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;

            _httpClient = new HttpClient();
        }

        // Index Action to retrieve the dashboard data
        public async Task<IActionResult> Index()
        {
            var aboutUs = await GetAboutUsAsync();
            var events = await GetEventsAsync();
            var model = new DashboardViewModel
            {
                AboutUsContent = aboutUs?.Content ?? "Default About Us content",
                MissionContent = aboutUs?.Mission ?? "Default Mission content",
                ImagePath = aboutUs?.ImagePath,
                TotalDonations = await GetTotalDonationsAsync(),
                Events = events,
                Locations = await GetLocationsAsync(),
            };
            return View(model);
        }

        private async Task<AboutUs> GetAboutUsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/aboutUs.json");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"About Us JSON: {json}"); // Log the raw JSON response

                // Attempt to deserialize the JSON into the AboutUs model
                try
                {
                    var aboutUsDictionary = JsonConvert.DeserializeObject<Dictionary<string, AboutUs>>(json);
                    return aboutUsDictionary?.Values.FirstOrDefault(); // Get the first value if exists
                }
                catch (JsonSerializationException ex)
                {
                    _logger.LogError($"JSON Deserialization Error: {ex.Message}");
                }
            }
            _logger.LogError("Failed to retrieve About Us data");
            return null;
        }


        private async Task<List<Events>> GetEventsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/events.json");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var eventsDictionary = JsonConvert.DeserializeObject<Dictionary<string, Events>>(json);

                // Check if eventsDictionary is null or empty
                if (eventsDictionary == null || !eventsDictionary.Any())
                {
                    _logger.LogWarning("No events found or eventsDictionary is null.");
                    return new List<Events>(); // Return an empty list
                }

                // Add EventId to each Event object
                return eventsDictionary.Select(e =>
                {
                    e.Value.eventId = e.Key; // Set the EventId from the key
                    return e.Value;
                }).ToList();
            }

            // Log if the response was not successful
            _logger.LogError($"Failed to retrieve events. Status Code: {response.StatusCode}");
            return new List<Events>(); // Return an empty list if there's an error
        }
    


    private async Task<List<Location>> GetLocationsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/locations.json");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var locationsDictionary = JsonConvert.DeserializeObject<Dictionary<string, Location>>(json);

                // Add LocationId to each Location object
                return locationsDictionary.Select(l =>
                {
                    l.Value.LocationId = l.Key; // Set the LocationId from the key
                    return l.Value;
                }).ToList();
            }
            return new List<Location>(); // Return an empty list if there's an error
        }



        // Get Total Donations
        private async Task<decimal> GetTotalDonationsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/donations.json");
            decimal totalDonations = 0;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var donationData = JsonConvert.DeserializeObject<Dictionary<string, Donation>>(json);
                if (donationData != null)
                {
                    foreach (var donation in donationData.Values)
                    {
                        totalDonations += donation.Amount;
                    }
                }
            }
            return totalDonations;
        }
        [HttpPost]
        public async Task<IActionResult> AddEvent(string eventName, DateTime eventDate, string eventDescription, List<IFormFile> eventImage)
        {
            var newEvent = new Events
            {
                eventName = eventName,
                date = eventDate,
                description = eventDescription,
                imageUrl = await ProcessUploadedFiles(eventImage, "events") // Process and upload images
            };

            var json = JsonConvert.SerializeObject(newEvent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/events.json", content);

            if (response.IsSuccessStatusCode)
            {
                // Retrieve the ID from the response
                var responseContent = await response.Content.ReadAsStringAsync();
                var eventId = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent).Keys.FirstOrDefault();

                // Assign the ID back to the new location
                newEvent.eventId = eventId;

                return RedirectToAction("Index");
            }

            // Log error and return a bad request
            _logger.LogError("Failed to add event");
            return BadRequest("Failed to add event");
        }

        

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                _logger.LogError("Event ID is null or empty");
                return BadRequest("Invalid event ID");
            }

            // Perform the delete operation on the specific event
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/events/{eventId}.json");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            // Log error and return a bad request
            _logger.LogError($"Failed to delete events with ID {eventId}");
            return BadRequest("Failed to delete event");
        }

       
        // Add About Us
        [HttpPost]
        public async Task<IActionResult> EditAboutUs(string aboutUsContent, string missionContent)
        {
            var aboutUs = new AboutUs { Content = aboutUsContent, Mission = missionContent };

            var json = JsonConvert.SerializeObject(aboutUs);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{BaseUrl}/aboutUs.json", content); // Use PUT to overwrite

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            // Log error and return a bad request
            _logger.LogError("Failed to edit About Us");
            return BadRequest("Failed to edit About Us");
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(string locationName, string locationAddress)
        {
            var newLocation = new Location
            {
                addressName = locationName,
                address = locationAddress
            };

            var json = JsonConvert.SerializeObject(newLocation);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/locations.json", content);

            if (response.IsSuccessStatusCode)
            {
                // Retrieve the ID from the response
                var responseContent = await response.Content.ReadAsStringAsync();
                var locationId = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent).Keys.FirstOrDefault();

                // Assign the ID back to the new location
                newLocation.LocationId = locationId;

                // Here, you may want to save the location with the ID if you have a method for that
                // For example, save it in your in-memory list or update the model

                return RedirectToAction("Index");
            }

            // Log error and return a bad request
            _logger.LogError("Failed to add location");
            return BadRequest("Failed to add location");
        }



        [HttpPost]
        public async Task<IActionResult> DeleteLocation(string locationId)
        {
            if (string.IsNullOrWhiteSpace(locationId))
            {
                _logger.LogError("Location ID is null or empty");
                return BadRequest("Invalid location ID");
            }

            // Perform the delete operation on the specific location
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/locations/{locationId}.json");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            // Log error and return a bad request
            _logger.LogError($"Failed to delete location with ID {locationId}");
            return BadRequest("Failed to delete location");
        }






        private async Task<string> ProcessUploadedFiles(List<IFormFile> files, string folderName)
        {
            string imagePath = null;
            if (files != null && files.Count > 0)
            {
                var directoryPath = Path.Combine(_hostingEnvironment.WebRootPath, folderName);
                // Create directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var uploadPath = Path.Combine(directoryPath, fileName);
                        using (var stream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        imagePath = $"{folderName}/{fileName}"; // Store relative path
                    }
                }
            }
            return imagePath;
        }
    }
    }
