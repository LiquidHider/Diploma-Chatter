using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.DataAccess.Models.Parameters
{
    internal class GroupChatListParameters
    {
        public SortOrder SortOrder { get; set; }

        public GroupChatSort SortBy { get; set; }
    }
}
