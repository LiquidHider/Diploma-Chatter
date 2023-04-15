using Chatter.Domain.BusinessLogic.Models.Abstract;

namespace Chatter.Domain.BusinessLogic.Models.Create
{
    public class CreateChatMessage
    {
        public Guid SenderID { get; set; }

        public Guid RecipientID { get; set; }

        public string Body { get; set; }
    }
}
