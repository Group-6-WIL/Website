using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wil_v3.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Wil_v3.Controllers
{
    public class ContactController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://ladybird-foundation-default-rtdb.firebaseio.com/";

        public ContactController()
        {
            _httpClient = new HttpClient();
        }

        // Get all contacts and display them
        public async Task<IActionResult> Index()
        {
            var contacts = await GetContactsAsync(); // Fetch all contacts from Firebase

            // Populate the model for the view
            var model = new DashboardViewModel
            {
                TeamContacts = contacts
            };

            return View(model); // Return the contacts view
        }

        // Add or update contact
        [HttpPost]
        public async Task<IActionResult> EditContact(string name, string email, string number, IFormFile contactImage)
        {
            // Process uploaded image file and get the unique file name (or URL if stored on Firebase storage)
            string uniqueFileName = await ProcessUploadedFileAsync(contactImage);

            // Create the contact object
            var contact = new Contact
            {
                name = name,
                email = email,
                number = number,
                imageUrl = uniqueFileName
            };

            // Check if the contact already exists (using email as the unique identifier)
            var existingContacts = await GetContactsAsync();
            var existingContact = existingContacts.Find(c => c.email == email); // Assuming email is unique for each contact

            if (existingContact != null)
            {
                // If contact exists, update it
                contact.id = existingContact.id; // Retain the existing Firebase ID
                await UpdateContactAsync(contact); // Update contact
            }
            else
            {
                // If contact doesn't exist, create a new one
                await AddContactAsync(contact); // Add contact
            }

            return RedirectToAction("Index"); // Redirect to the Index action after updating/adding contact
        }

        // Get all contacts from Firebase
        private async Task<List<Contact>> GetContactsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/contacts.json"); // Firebase GET request

                // Log response status
                Console.WriteLine($"Firebase GET Response Status: {response.StatusCode}");

                // If the request is successful
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(); // Get JSON response

                    // Log the raw JSON received from Firebase
                    Console.WriteLine($"Raw JSON from Firebase: {json}");

                    var contactsData = JsonConvert.DeserializeObject<Dictionary<string, Contact>>(json); // Deserialize into dictionary
                    if (contactsData != null)
                    {
                        // Assign the Firebase key as the Contact ID and return the list of contacts
                        return contactsData.Select(c => { c.Value.id = Convert.ToInt32(c.Key); return c.Value; }).ToList();
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to get contacts from Firebase. Status Code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error fetching contacts: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON error deserializing contacts: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching contacts: {ex.Message}");
            }

            return new List<Contact>(); // Return empty list if there was an error
        }


        // Add a new contact to Firebase
        private async Task AddContactAsync(Contact contact)
        {
            var json = JsonConvert.SerializeObject(contact); // Serialize the contact object to JSON
            var contentString = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"{BaseUrl}/contacts.json", contentString); // POST request to add a new contact
        }

        // Update an existing contact in Firebase
        private async Task UpdateContactAsync(Contact contact)
        {
            var json = JsonConvert.SerializeObject(contact); // Serialize contact object to JSON
            var contentString = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Use the contact's ID to update in Firebase
            await _httpClient.PutAsync($"{BaseUrl}/contacts/{contact.id}.json", contentString); // PUT request to update contact by ID
        }

        // Process uploaded image and return unique file name or URL
        private async Task<string> ProcessUploadedFileAsync(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                // Logic for processing file, for now using unique file name (replace with actual Firebase Storage logic if needed)
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                // Implement file upload logic (e.g., to Firebase Storage)
            }

            return uniqueFileName;
        }
    }
}
