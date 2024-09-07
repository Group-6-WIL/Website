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
                TeamContacts = _context.Contacts.ToList() // Changed to the correct property name 'Contacts'
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditContact(int id, string teamMember, string email, string phone, List<IFormFile> teamMemberImage)
        {
            List<string> imagePaths = ProcessUploadedFiles(teamMemberImage, "team");

            var contact = _context.Contacts.FirstOrDefault(c => c.Id == id);
            if (contact != null)
            {
                contact.TeamMember = teamMember;
                contact.Email = email;
                contact.Phone = phone;
                if (imagePaths.Count > 0)
                {
                    contact.ImagePath = string.Join(";", imagePaths);
                }
                _context.Contacts.Update(contact);
                _context.SaveChanges();
            }
            else
            {
                _logger.LogError($"Contact with ID {id} not found.");
                return NotFound();
            }

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
