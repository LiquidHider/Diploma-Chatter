using Chatter.Web.Enums;

namespace Chatter.Web.Models
{
    public class ContactsViewModel
    {
        public PaginatedResponse<ChatUser, ChatUserSort> PaginatedContactsList { get; set; }

        public ChatUser CurrentChatUser { get; set; }
    }
}
