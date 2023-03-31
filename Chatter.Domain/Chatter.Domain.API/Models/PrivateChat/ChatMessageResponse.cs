
namespace Chatter.Domain.API.Models.PrivateChat
{
    public class ChatMessageResponse
    {
        public Guid ID { get; set; }

        public bool IsRead { get; set; }

        public bool IsEdited { get; set; }

        public string Body { get; set; }

        public DateTime Sent { get; set; }

        public Guid Sender { get; set; }

        public Guid RecipientID { get; set; }
    }
}
