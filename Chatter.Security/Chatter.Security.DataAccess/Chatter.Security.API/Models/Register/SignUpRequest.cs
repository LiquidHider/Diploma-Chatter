namespace Chatter.Security.API.Models.Register
{
    public class SignUpRequest
    {
        public string? Email { get; set; }

        public string? UserTag { get; set; }

        public string Password { get; set; }

        public Guid UserId { get; set; }
    }
}
