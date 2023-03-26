namespace Chatter.Domain.DataAccess.Models
{
    public class UpdateChatUserModel
    {
        public Guid ID { get; set; }
        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? Patronymic { get; set; }

        public string? UniversityName { get; set; }

        public string? UniversityFaculty { get; set; }

        public DateTime? LastActive { get; set; }

        public bool? IsBlocked { get; set; }

        public DateTime? BlockedUntil { get; set; }
    }
}
