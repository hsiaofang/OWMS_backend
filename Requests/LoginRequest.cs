using System.Text.Json.Serialization;

namespace OWMS.Requests
{
    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
