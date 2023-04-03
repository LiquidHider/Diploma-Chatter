namespace Chatter.Security.DataAccess.Models
{
    public class CreateIdentity
    {
        public Guid ID { get; set; }

        public string Email { get; set; }

        public string UserTag { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordKey { get; set; }

        public Guid UserID { get; set; }
    }
}
