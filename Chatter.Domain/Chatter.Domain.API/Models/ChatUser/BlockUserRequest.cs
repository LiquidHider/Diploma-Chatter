namespace Chatter.Domain.API.Models.ChatUser
{
    public class BlockUserRequest
    {
        public Guid UserID { get; set; }

        public DateTime? BlockedUntilUtc { get; set; }
    }
}
