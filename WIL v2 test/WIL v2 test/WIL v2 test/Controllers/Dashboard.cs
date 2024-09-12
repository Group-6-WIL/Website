using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WIL_v2_test.Data;
using WIL_v2_test.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace WIL_v2_test.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<DashboardController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var aboutUs = _context.AboutUs.FirstOrDefault();
            var model = new DashboardViewModel
            {
                AboutUsContent = aboutUs?.Content ?? "Default About Us content",
                MissionContent = aboutUs?.Mission ?? "Default Mission content",
                ImagePath = aboutUs?.ImagePath,
                TotalDonations = _context.Donations.Sum(d => d.Amount),
                Events = _context.Events.ToList(),
                Locations = _context.Locations.ToList(), // Ensure Locations is initialized
                TeamContacts = _context.Contacts.ToList() // Changed to the correct property name 'Contacts'
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult AddEvent(string eventName, DateTime eventDate, string eventDescription, List<IFormFile> eventImage)
        {
            List<string> imagePaths = ProcessUploadedFiles(eventImage, "events");

            var newEvent = new Events
            {
                Name = eventName,
                Date = eventDate,
                Description = eventDescription,
                ImagePath = string.Join(";", imagePaths)
            };
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditEvent(int id, string eventName, DateTime eventDate, string eventDescription, List<IFormFile> eventImage)
        {
            // Fetch the event to be updated
            var eventToUpdate = _context.Events.FirstOrDefault(e => e.Id == id);
            if (eventToUpdate == null)
            {
                // Log the error and return a 404 response if the event is not found
                _logger.LogError($"Event with id {id} not found.");
                return NotFound();
            }

            // Process and assign image paths
            List<string> imagePaths = ProcessUploadedFiles(eventImage, "events");
            if (imagePaths.Any())
            {
                // Assuming you want to store multiple images, concatenate paths or handle as needed
                eventToUpdate.ImagePath = string.Join(",", imagePaths);
            }

            // Update event details
            eventToUpdate.Name = eventName;
            eventToUpdate.Date = eventDate;
            eventToUpdate.Description = eventDescription;

            // Save changes to the database
            _context.Events.Update(eventToUpdate);
            _context.SaveChanges();

            // Redirect to the index page or another appropriate action
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            var eventToDelete = _context.Events.Find(id);
            if (eventToDelete == null)
            {
                _logger.LogError($"Event with id {id} not found.");
                return NotFound();
            }

            _context.Events.Remove(eventToDelete);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult EventDetails(int id)
        {
            var eventDetails = _context.Events.Find(id);
            if (eventDetails == null)
            {
                _logger.LogError($"Event with id {id} not found.");
                return NotFound();
            }

            return View("~/Views/Events/Details.cshtml", eventDetails); // Specify the correct view path
        }


        [HttpPost]
        public IActionResult EditAboutUs(string aboutUsContent, string missionContent)
        {


            var aboutUs = _context.AboutUs.FirstOrDefault();
            if (aboutUs != null)
            {
                aboutUs.Content = aboutUsContent;
                aboutUs.Mission = missionContent;

            }
            else
            {
                _context.AboutUs.Add(new AboutUs { Content = aboutUsContent, Mission = missionContent});
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddLocation(string locationName, string locationAddress)
        {
            var newLocation = new Location
            {
                Name = locationName,
                Address = locationAddress
            };
            _context.Locations.Add(newLocation);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Redirect to Locations page
        }

        public IActionResult Locations()
        {
            var locations = _context.Locations.ToList(); // Retrieve all locations from the database
            return View(locations); // Pass locations to the view
        }

        [HttpPost]
        public IActionResult EditLocation(int id, string locationName, string locationAddress)
        {
            var locationToUpdate = _context.Locations.FirstOrDefault(l => l.Id == id);
            if (locationToUpdate == null)
            {
                _logger.LogError($"Location with id {id} not found.");
                return NotFound();
            }

            locationToUpdate.Name = locationName;
            locationToUpdate.Address = locationAddress;

            _context.Locations.Update(locationToUpdate);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteLocation(int id)
        {
            var locationToDelete = _context.Locations.Find(id);
            if (locationToDelete == null)
            {
                _logger.LogError($"Location with id {id} not found.");
                return NotFound();
            }

            _context.Locations.Remove(locationToDelete);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult EditContact(int id, string teamMember, string email, string phone, List<IFormFile> teamMemberImage)
        {
            List<string> imagePaths = ProcessUploadedFiles(teamMemberImage, "team");

            var contact = _context.Contacts.FirstOrDefault(c => c.Id == id);
            if (contact != null)
            {
                // Update existing contact
                contact.TeamMember = teamMember;
                contact.Email = email;
                contact.Phone = phone;
                if (imagePaths.Count > 0)
                {
                    contact.ImagePath = string.Join(";", imagePaths);
                }
                _context.Contacts.Update(contact);
            }
            else
            {
                // Add new contact
                contact = new Contact
                {
                    TeamMember = teamMember,
                    Email = email,
                    Phone = phone,
                    ImagePath = string.Join(";", imagePaths)
                };
                _context.Contacts.Add(contact);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult DeleteContact(int id)
        {
            var contactToDelete = _context.Contacts.Find(id);
            if (contactToDelete == null)
            {
                _logger.LogError($"Contact with ID {id} not found.");
                return NotFound();
            }

            _context.Contacts.Remove(contactToDelete);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Other methods (AddEvent, EditAboutUs, etc.) remain unchanged

        private List<string> ProcessUploadedFiles(List<IFormFile> files, string folderName)
        {
            List<string> fileNames = new List<string>();

            if (files != null && files.Count > 0)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, folderName);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in files)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    fileNames.Add(uniqueFileName);
                }
            }

            return fileNames;
        }
    }
}
