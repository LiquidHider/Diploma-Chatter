namespace Chatter.Web.Models
{
    public class User
    {
        public Guid ID { get; set; }

        public List<string> Roles { get; set; }
    }
}
