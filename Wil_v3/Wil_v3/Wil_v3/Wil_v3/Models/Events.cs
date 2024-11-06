using System.Globalization;
using System.Text.Json.Serialization;

namespace Wil_v3.Models
{
    public class Events
    {
        public DateTime date { get; set; }
        public string description { get; set; }
        public string eventId { get; set; } // Adjusted to match your field names
        public string eventName { get; set; }
        public string imageUrl { get; set; }

    }
}
