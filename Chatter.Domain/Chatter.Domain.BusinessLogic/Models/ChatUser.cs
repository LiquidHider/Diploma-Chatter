using Chatter.Domain.BusinessLogic.Models.Abstract;

namespace Chatter.Domain.BusinessLogic.Models
{
    public class ChatUser : User
    {
        public string Email { get; set; }

        public string UniversityName { get; set; }

        public string UniversityFaculty { get; set; }

        public DateTime JoinedUtc { get; set; }

        public DateTime LastActiveUtc { get; set; }

        public List<Photo> Photos { get; set; }

        public List<Guid> Contacts { get; set; }

        public bool IsBlocked { get; set; }

        public DateTime? BlockedUntilUtc { get; set; }
    }
}
 