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
                ImagePath = aboutUs?.ImagePath, // Include this if you have an image path in the database
                TotalDonations = _context.Donations.Sum(d => d.Amount),
                Events = _context.Events.ToList(),
                TeamContacts = _context.TeamContacts.ToList() // Add this line
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
        public IActionResult EditAboutUs(string aboutUsContent, string missionContent, List<IFormFile> aboutUsImage)
        {
            List<string> imagePaths = ProcessUploadedFiles(aboutUsImage, "images");

            var aboutUs = _context.AboutUs.FirstOrDefault();
            if (aboutUs != null)
            {
                aboutUs.Content = aboutUsContent;
                aboutUs.Mission = missionContent;
                aboutUs.ImagePath = string.Join(";", imagePaths);
            }
            else
            {
                _context.AboutUs.Add(new AboutUs { Content = aboutUsContent, Mission = missionContent, ImagePath = string.Join(";", imagePaths) });
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

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditContact(string teamMember, string email, string phone, List<IFormFile> teamMemberImage)
        {
            List<string> imagePaths = ProcessUploadedFiles(teamMemberImage, "team");

            var contact = _context.Contacts.FirstOrDefault(c => c.TeamMember == teamMember);
            if (contact != null)
            {
                contact.Email = email;
                contact.Phone = phone;
                contact.ImagePath = string.Join(";", imagePaths);
            }
            else
            {
                _context.Contacts.Add(new Contact
                {
                    TeamMember = teamMember,
                    Email = email,
                    Phone = phone,
                    ImagePath = string.Join(";", imagePaths)
                });
            }
            _context.SaveChanges();

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
