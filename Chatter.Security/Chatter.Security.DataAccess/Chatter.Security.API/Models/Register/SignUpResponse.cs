namespace Chatter.Security.API.Models.Register
{
    public class SignUpResponse
    {
        public Guid UserID { get; set; }

        public string Token { get; set; }
    }
}
