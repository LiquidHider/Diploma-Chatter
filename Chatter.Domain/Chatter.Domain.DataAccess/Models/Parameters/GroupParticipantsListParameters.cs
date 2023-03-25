using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.DataAccess.Models.Parameters
{
    public class GroupParticipantsListParameters
    {
        public Guid GroupID { get; set; }

        public SortOrder SortOrder { get; set; }

        public GroupParticipantSort SortBy { get; set; }

        public bool? ShowBlocked { get; set; }
    }
}
