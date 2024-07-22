using Microsoft.AspNetCore.Mvc;
using WIL_v2_test.Data;
using WIL_v2_test.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace WIL_v2_test.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ContactController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var contact = _context.Contacts.FirstOrDefault();
            var model = new DashboardViewModel
            {
                TeamMember = contact?.TeamMember ?? "Default Team Member",
                Email = contact?.Email ?? "default@example.com",
                Phone = contact?.Phone ?? "000-000-0000",
                ContactUsImagePath = contact?.ImagePath
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditContact(string teamMember, string email, string phone, IFormFile teamMemberImage)
        {
            string uniqueFileName = ProcessUploadedFile(teamMemberImage);

            var contact = _context.Contacts.FirstOrDefault();
            if (contact != null)
            {
                contact.TeamMember = teamMember;
                contact.Email = email;
                contact.Phone = phone;
                if (!string.IsNullOrEmpty(uniqueFileName))
                {
                    contact.ImagePath = uniqueFileName;
                }
            }
            else
            {
                _context.Contacts.Add(new Contact
                {
                    TeamMember = teamMember,
                    Email = email,
                    Phone = phone,
                    ImagePath = uniqueFileName
                });
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private string ProcessUploadedFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "team");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}





