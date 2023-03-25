namespace Chatter.Domain.DataAccess.Models
{
    public class GroupChatBlockModel
    {
        public Guid ID { get; set; 
        }
        public Guid GroupID { get; set; }

        public Guid UserID { get; set; }

        public DateTime? BlockedUntil { get; set; }
    }
}
