using Chatter.Domain.BusinessLogic.Models.ChatMessages;
using Chatter.Domain.BusinessLogic.Models.Abstract;

namespace Chatter.Domain.BusinessLogic.Models.Chats
{
    public class PrivateChat
    {
        public User Member1 { get; set; }

        public User Member2 { get; set; }
    }
}
