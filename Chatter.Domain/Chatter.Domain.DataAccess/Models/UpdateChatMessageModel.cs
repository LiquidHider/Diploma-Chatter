namespace Chatter.Domain.DataAccess.Models
{
    public class UpdateChatMessageModel
    {
        public Guid ID { get; set; }

        public string? Body { get; set; }

        public bool IsEdited { get; set; }

        public bool? IsRead { get; set; }
    }
}
