using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.DataAccess.Models.Parameters
{
    public class GroupChatListParameters
    {
        public SortOrder SortOrder { get; set; }

        public GroupChatSort SortBy { get; set; }

        public Guid? UserId { get; set; }
    }
}
