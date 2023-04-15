namespace Chatter.Domain.API.Models.PrivateChat
{
    public class CreateChatMessageRequest
    {
        public string ConnectionID { get; set; }

        public Guid SenderID { get; set; }

        public Guid RecipientID { get; set; }

        public DateTime Sent { get; set; }

        public string Body { get; set; }
    }
}
