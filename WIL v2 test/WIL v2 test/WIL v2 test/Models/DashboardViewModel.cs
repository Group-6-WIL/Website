using NUnit.Framework.Internal.Execution;

namespace WIL_v2_test.Models
{
    public class DashboardViewModel
    {
        public string AboutUsContent { get; set; }
        public string MissionContent { get; set; }
        public string ImagePath { get; set; }
        public decimal TotalDonations { get; set; }

        // Contact Us properties

        public string TeamMember { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactUsImagePath { get; set; }

        // Events properties
       // public List<Event> Events { get; set; }
        public List<WIL_v2_test.Models.Events> Events { get; set; }
        public List<Location> Locations { get; set; }

        public IEnumerable<TeamContact> TeamContacts { get; set; } // Add this line
    }
}
