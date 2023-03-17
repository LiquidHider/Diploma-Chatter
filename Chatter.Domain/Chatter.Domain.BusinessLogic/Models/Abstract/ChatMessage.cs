namespace Chatter.Domain.BusinessLogic.Models.Abstract
{
    internal abstract class ChatMessage
    {
        public Guid ID { get; set; }

        public bool IsRead { get; set; }

        public User Sender { get; set; }

    }
}
