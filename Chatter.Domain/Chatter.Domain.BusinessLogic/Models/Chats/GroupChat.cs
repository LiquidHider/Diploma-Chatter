using Chatter.Domain.Common.Enums;
using Chatter.Domain.BusinessLogic.Models.Abstract;

namespace Chatter.Domain.BusinessLogic.Models.Chats
{
    public class GroupChat
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<ChatUser, GroupUserRole> Participants { get; set; }

        public List<ChatUser> BlockedUsers { get; set; }

        public List<ChatMessage> Messages { get; set; }
    }
}
