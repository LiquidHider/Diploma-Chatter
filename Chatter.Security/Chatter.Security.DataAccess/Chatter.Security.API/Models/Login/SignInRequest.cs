namespace Chatter.Security.API.Models.Login
{
    public class SignInRequest
    {
        public string? Email { get; set; }

        public string? UserTag { get; set; }

        public string Password { get; set; }
    }
}
