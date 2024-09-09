using Microsoft.AspNetCore.Mvc;
using WIL_v2_test.Data;
using WIL_v2_test.Models;
using System.Linq;

namespace WIL_v2_test.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutUsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var aboutUs = _context.AboutUs.FirstOrDefault();
            var model = new DashboardViewModel
            {
                AboutUsContent = aboutUs?.Content ?? "Default About Us content",
                MissionContent = aboutUs?.Mission ?? "Default Mission content",
                ImagePath = aboutUs?.ImagePath,
                LastUpdated = DateTime.Now // Set the current time
            };
            return View(model);
        }
    }
}
