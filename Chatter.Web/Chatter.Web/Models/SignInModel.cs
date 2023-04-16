namespace Chatter.Web.Models
{
    public class SignInModel
    {
        public string EmailOrUserTag { get; set; }

        public string Password { get; set; }

        public bool WrongCreds { get; set; } = false;

    }
}
