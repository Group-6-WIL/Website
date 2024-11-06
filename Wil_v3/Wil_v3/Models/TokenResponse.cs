namespace Wil_v3.Models
{
    public class TokenResponse
    {
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
        public string LocalId { get; set; }
        public bool IsNewUser { get; set; }
    }
}
