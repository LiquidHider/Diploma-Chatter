namespace Chatter.Security.API.Models.Login
{
    public class SignInResponse
    {
        public Guid UserID { get; set; }

        public string Token { get; set; }
    }
}
