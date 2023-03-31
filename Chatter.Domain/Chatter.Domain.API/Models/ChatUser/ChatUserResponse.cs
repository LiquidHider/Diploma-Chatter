using Chatter.Domain.API.Models.PrivateChat;

namespace Chatter.Domain.API.Models.ChatUser
{
    public class ChatUserResponse
    {
        public Guid ID { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string? Patronymic { get; set; }

        public DateTime JoinedUtc { get; set; }

        public DateTime LastActiveUtc { get; set; }

        public bool IsBlocked { get; set; }

        public DateTime? BlockedUntilUtc { get; set; }

        public string? UniversityName { get; set; }

        public string? UniversityFaculty { get; set; }

        public List<PrivateChatReponse> Contacts { get; set; }
    }
}
