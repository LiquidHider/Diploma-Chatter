using Chatter.Web.Interfaces;
using Chatter.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;

namespace Chatter.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly Uri _securityUri;
        private readonly Uri _domainUri;

        public AccountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _securityUri = new Uri(_configuration.GetSection("APIs:Security").Value);
            _domainUri = new Uri(_configuration.GetSection("APIs:Domain").Value);
        }

        public async Task<HttpResponseMessage> SignIn(SignInModel signInModel)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = _securityUri;

                var isLoginEmail = IsValueEmail(signInModel.EmailOrUserTag);

                string email = isLoginEmail ? signInModel.EmailOrUserTag : null;
                string userTag = !isLoginEmail ? signInModel.EmailOrUserTag : null;

                var request =
                    new { UserTag = userTag,
                        Email = email,
                        Password = signInModel.Password };

                var json = JsonConvert.SerializeObject(request);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await http.PostAsync("signIn", content);

                return response;
            }
        }

        public async Task<HttpResponseMessage> SignUp(SignUpModel signUpModel)
        {
            HttpResponseMessage securityResponse = null;

            using (var securityHttp = new HttpClient())
            {
                securityHttp.BaseAddress = _securityUri;

                var userExistsResponse = await securityHttp.PostAsync($"exists/?email={signUpModel.Email},userTag={signUpModel.UserTag}", null);
                var userExistsResponseContent = await userExistsResponse.Content.ReadAsStringAsync();

                var isUserExists = JsonConvert.DeserializeObject<bool>(userExistsResponseContent);

                if (isUserExists)
                {
                    return userExistsResponse;
                }

                string createdChatUserId = null;

                using (var domainHttp = new HttpClient())
                {
                    domainHttp.BaseAddress = _domainUri;

                    var domainRequestBody = new
                    {
                        LastName = signUpModel.LastName,
                        FirstName = signUpModel.FirstName,
                        Patronymic = signUpModel.Patronymic,
                        UniversityName = signUpModel.UniversityName,
                        UniversityFaculty = signUpModel.UniversityFaculty
                    };

                    var domainJson = JsonConvert.SerializeObject(domainRequestBody);

                    var domainContent = new StringContent(domainJson, Encoding.UTF8, "application/json");

                    var domainResponse = await domainHttp.PostAsync("user/new", domainContent);

                    var domainResponseContent = await domainResponse.Content.ReadAsStringAsync();

                    createdChatUserId = JsonConvert.DeserializeObject<CreatedResponse>(domainResponseContent).CreatedId.ToString();
                }

                var securityRequestBody =
                  new
                  {
                      UserTag = signUpModel.UserTag,
                      Email = signUpModel.Email,
                      Password = signUpModel.Password,
                      UserID = createdChatUserId

                  };

                var securityJson = JsonConvert.SerializeObject(securityRequestBody);

                var securityContent = new StringContent(securityJson, Encoding.UTF8, "application/json");

                securityResponse = await securityHttp.PostAsync("signUp", securityContent);

              
                

                return securityResponse;

            }

        }

        private bool IsValueEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            else
            {
                return Regex.IsMatch(email, pattern);
            }
        }

    }
}

