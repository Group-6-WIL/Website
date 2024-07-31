using System.Collections.Generic;

namespace WIL_v2_test.Models
{
    public class DashboardViewModel
    {
        public string AboutUsContent { get; set; }
        public string MissionContent { get; set; }
        public string ImagePath { get; set; } // Ensure this property is present
        public decimal TotalDonations { get; set; }

        // Contact Us properties
        public string TeamMember { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactUsImagePath { get; set; } // Ensure this property is present

        // Events properties
        public List<Events> Events { get; set; }
        public List<Location> Locations { get; set; }
        public IEnumerable<TeamContact> TeamContacts { get; set; }
    }
}
