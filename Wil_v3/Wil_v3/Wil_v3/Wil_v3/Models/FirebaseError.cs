namespace Wil_v3.Models // Ensure this matches your project's namespace
{
    public class FirebaseError
    {
        public ErrorDetails Error { get; set; }

        public class ErrorDetails
        {
            public string Code { get; set; }
            public string Message { get; set; }
        }
    }
}
