using Chatter.Domain.BusinessLogic.Models.ChatMessages;
using Chatter.Domain.BusinessLogic.Models.Abstract;

namespace Chatter.Domain.BusinessLogic.Models.Chats
{
    public class PrivateChat
    {
        public Guid Member1ID { get; set; }

        public Guid Member2ID { get; set; }
    }
}
