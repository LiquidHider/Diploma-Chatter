namespace Chatter.Domain.DataAccess.Models
{
    public class ChatMessageModel
    {
        public Guid ID { get; set; }

        public string Body { get; set; }

        public bool IsEdited { get; set; }

        public DateTime Sent { get; set; }

        public bool IsRead { get; set; }

        public Guid SenderId { get; set; }

        public Guid? RecipientUserId { get; set; }

        public Guid? RecipientGroupId { get; set; }
    }
}
