using System.Text.Json.Serialization;

namespace OWMS.Requests
{
    public class AuthRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
