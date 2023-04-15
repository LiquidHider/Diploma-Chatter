namespace Chatter.Web.Models
{
    public class SignUpModel
    {
        public string UserTag { get; set; }

        public string Email { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string? Patronymic { get; set; }

        public string? UniversityName { get; set; }

        public string? UniversityFaculty { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

    }
}
