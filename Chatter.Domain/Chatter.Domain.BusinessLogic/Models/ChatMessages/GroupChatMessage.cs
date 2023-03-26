using Chatter.Domain.BusinessLogic.Models.Abstract;
using Chatter.Domain.BusinessLogic.Models.Chats;

namespace Chatter.Domain.BusinessLogic.Models.ChatMessages
{
    public class GroupChatMessage : ChatMessage
    {
        public GroupChat Recipient { get; set; }
    }
}
