namespace Chatter.Domain.BusinessLogic.Models.Update
{
    public class UpdateChatUser
    {
        public Guid UserID { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? Patronymic { get; set; }

        public string? UniversityName { get; set; }

        public string? UniversityFaculty { get; set; }
    }
}
