namespace Chatter.Domain.DataAccess.Models
{
    public class UpdateGroupChat
    {
        public Guid ID { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
