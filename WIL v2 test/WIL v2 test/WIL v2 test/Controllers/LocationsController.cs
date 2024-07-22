using Microsoft.AspNetCore.Mvc;
using WIL_v2_test.Data;
using WIL_v2_test.Models;
using System.Linq;

namespace WIL_v2_test.Controllers
{
    public class LocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var locations = _context.Locations.ToList();
            return View(locations);
        }
    }
}
