using Chatter.Web.Models;

namespace Chatter.Web.Interfaces
{
    public interface IAccountService
    {
        Task<HttpResponseMessage> SignIn(SignInModel signInModel);

        Task<HttpResponseMessage> SignUp(SignUpModel signUpModel);
    }
}
