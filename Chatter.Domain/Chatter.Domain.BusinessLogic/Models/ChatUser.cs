using Chatter.Domain.BusinessLogic.Models.Abstract;
using Chatter.Domain.BusinessLogic.Models.Chats;

namespace Chatter.Domain.BusinessLogic.Models
{
    public class ChatUser : User
    {
        public string? UniversityName { get; set; }

        public string? UniversityFaculty { get; set; }

        public List<PrivateChat> Contacts { get; set; }
    }
}
 