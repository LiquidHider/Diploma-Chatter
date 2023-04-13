using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.BusinessLogic.Models.Parameters
{
    public class ChatUserListParameters
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public SortOrder SortOrder { get; set; }

        public ChatUserSort SortBy { get; set; }

        public List<Guid>? Users { get; set; }

        public List<string>? UniversitiesNames { get; set; }

        public List<string>? UniversitiesFaculties { get; set; }

    }
}
