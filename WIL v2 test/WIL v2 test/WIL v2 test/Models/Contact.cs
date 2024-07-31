namespace WIL_v2_test.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string TeamMember { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImagePath { get; set; } // Change this to string to store multiple paths
    }
}
