using Chatter.Domain.BusinessLogic.Models.Abstract;

namespace Chatter.Domain.BusinessLogic.Models.ChatMessages
{
    internal class PrivateChatMessage : ChatMessage
    {
        public ChatUser Recipient { get; set; }
    }
}
