namespace Wil_v3.Models
{
    public class Location
    {
        public string LocationId { get; set; } // Unique ID
        public string address { get; set; }
        public string addressName { get; set; }

        public string suburb { get; set; } // New property for the area
    }
}
