using Microsoft.AspNetCore.Mvc;
using WIL_v2_test.Data;
using WIL_v2_test.Models;
using System.Linq;

namespace WIL_v2_test.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var donations = _context.Donations.ToList();
            return View(donations);
        }

        [HttpPost]
        public IActionResult AddDonation(decimal amount)
        {
            var donation = new Donation { Amount = amount };
            _context.Donations.Add(donation);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

