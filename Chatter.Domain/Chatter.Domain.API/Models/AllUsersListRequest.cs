using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.API.Models
{
    public class AllUsersListRequest
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public SortOrder SortOrder { get; set; }

        public ChatUserSort SortBy { get; set; }
    }
}
