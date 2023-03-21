namespace Chatter.Domain.DataAccess.Models
{
    public class UpdateGroupChatModel
    {
        public Guid ID { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
