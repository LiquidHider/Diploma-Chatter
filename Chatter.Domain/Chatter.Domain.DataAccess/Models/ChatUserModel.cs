namespace Chatter.Domain.DataAccess.Models
{
    public class ChatUserModel
    {
        public Guid ID { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Patronymic { get; set; }

        public string Email { get; set; }

        public string? UniversityName { get; set; }

        public string? UniversityFaculty { get; set; }

        public DateTime JoinedUtc { get; set; }

        public DateTime LastActiveUtc { get; set; }

        public bool IsBlocked { get; set; }

        public DateTime? BlockedUntilUtc { get; set; }
    }
}
