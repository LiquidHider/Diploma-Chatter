namespace Chatter.Security.DataAccess.Models
{
    public class UpdateIdentityModel
    {
        public Guid ID { get; set; }

        public string? Email { get; set; }

        public string? UserTag { get; set; }

        public string? Password { get; set; }
    }
}
