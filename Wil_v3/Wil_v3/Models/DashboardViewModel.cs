﻿using System.Collections.Generic;

namespace Wil_v3.Models
{
    public class DashboardViewModel
    {
        // About Us properties
        public string AboutUsContent { get; set; }
        public string MissionContent { get; set; }
        public string ImagePath { get; set; } // Path to the About Us image

        // Donation properties
        public decimal TotalDonations { get; set; }

        // Events and Locations properties
        public List<Events> Events { get; set; }
        public List<Location> Locations { get; set; }

        // Team Contacts properties
        public IEnumerable<Contact> TeamContacts { get; set; } // Change the type to Contact
        public List<Location> Location { get; set; } = new List<Location>();


        // Contact Us properties for editing a specific contact
        public string TeamMember { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactUsImagePath { get; set; } // Path to the contact image

        public DateTime LastUpdated { get; set; } // Add this property
    }
}
