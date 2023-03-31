namespace Chatter.Domain.API.Models.ChatUser
{
    public class UpdateChatUserRequest
    {
        public Guid UserID { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? Patronymic { get; set; }

        public string? UniversityName { get; set; }

        public string? UniversityFaculty { get; set; }
    }
}
