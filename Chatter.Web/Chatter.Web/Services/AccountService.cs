using Chatter.Web.Interfaces;
using Chatter.Web.Models;
using Newtonsoft.Json;
using System.Text;

namespace Chatter.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;

        public AccountService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> SignIn(SignInModel signInModel)
        {
            using (var http = new HttpClient()) 
            {
                http.BaseAddress = new Uri(_configuration.GetSection("APIs:Security").Value);
                var request = 
                    new { UserTag = signInModel.EmailOrUserTag, Password = signInModel.Password };

                var json = JsonConvert.SerializeObject(request);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await http.PostAsync("signIn",content);

                return response;
            }
        }

        public async Task<HttpResponseMessage> SignUp(SignUpModel signUpModel)
        {
            throw new NotImplementedException();
        }
    }
}
