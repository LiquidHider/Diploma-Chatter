namespace Chatter.Security.Core.Models
{
    public class CreateIdentity
    {
        public string Email { get; set; }

        public string? UserTag { get; set; }

        public string Password { get; set; }

        public Guid UserID { get; set; }
    }
}
