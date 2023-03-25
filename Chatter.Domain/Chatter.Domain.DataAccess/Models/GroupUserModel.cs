using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.DataAccess.Models
{
    public class GroupUserModel
    {
        public Guid ID { get; set; }

        public Guid GroupID { get; set; }

        public Guid UserID { get; set; }

        public GroupUserRole UserRole { get; set; }
    }
}
