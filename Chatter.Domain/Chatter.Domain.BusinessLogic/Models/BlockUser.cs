namespace Chatter.Domain.BusinessLogic.Models
{
    public class BlockUser
    {
        public Guid UserID { get; set; }
        public bool IsBlocked { get; private set; } = true;

        public DateTime BlockedUntilUtc { get; set; }
    }
}
