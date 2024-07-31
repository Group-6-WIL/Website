namespace WIL_v2_test.Models
{

    public class Events
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; } // Change this to string to store multiple paths
    }
}

