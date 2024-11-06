namespace Wil_v3.Models
{
    public class Contact
    {
        public string email { get; set; }
        public int id { get; set; }
        public string imageUrl { get; set; } // Change this to string to store multiple paths
        public string name { get; set; }

        public string number { get; set; }
    }
}
