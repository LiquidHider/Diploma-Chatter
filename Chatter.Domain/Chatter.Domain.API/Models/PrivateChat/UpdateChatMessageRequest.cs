namespace Chatter.Domain.API.Models.PrivateChat
{
    public class UpdateChatMessageRequest
    {
        public Guid ID { get; set; }

        public string? Body { get; set; }

        public bool? IsRead { get; set; }

        public bool IsEdited { get; set; }
    }
}
