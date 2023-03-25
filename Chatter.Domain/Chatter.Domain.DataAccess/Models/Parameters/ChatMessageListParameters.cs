using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.DataAccess.Models.Parameters
{
    public class ChatMessageListParameters
    {
        public SortOrder SortOrder { get; set; }

        public ChatMessageSort SortBy { get; set; }

        public bool RecipientIsGroup { get; set; }

        public Guid Sender { get; set; }

        public Guid Recipient { get; set; }
    }
}
