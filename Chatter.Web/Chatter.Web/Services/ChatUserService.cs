using Chatter.Web.Interfaces;
using Chatter.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Chatter.Web.Services
{
    public class ChatUserService : IChatUserService
    {
        private readonly Uri _domainBaseUri;
        private readonly IConfiguration _configuration;

        public ChatUserService(IConfiguration configuration)
        {
            _configuration = configuration;
            _domainBaseUri = new Uri(_configuration.GetSection("APIs:Domain").Value);
        }

        public async Task<HttpResponseMessage> GetChatUser(Guid id, string authToken)
        {
            using (HttpClient http = new HttpClient())
            {
                http.BaseAddress = _domainBaseUri;
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var getUserEndpoint = $"/user/?Id={id}";

                var response = await http.GetAsync(getUserEndpoint);

                return response;
            }
        }

        public async Task<HttpResponseMessage> GetAllUsersList(int pageNumber, int pageSize, string authToken) 
        {
            using (HttpClient http = new HttpClient())
            {
                http.BaseAddress = _domainBaseUri;
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var getUsersListEndpoint = $"/user/all";

                var request =
                  new
                  {
                      PageNumber = pageNumber,
                      PageSize = pageSize,
                      SortOrder = 0, //Asc
                      SortBy = 0 //Last name
                  };

                var json = JsonConvert.SerializeObject(request);

                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await http.PostAsync(getUsersListEndpoint, requestContent);

                return response;
            }
        }

        public async Task<HttpResponseMessage> GetChatUserContacts(Guid id, string authToken)
        {
            using (HttpClient http = new HttpClient())
            {
                http.BaseAddress = _domainBaseUri;
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var getUserEndpoint = $"/user/contacts/?userId={id}";

                var response = await http.PostAsync(getUserEndpoint,null);

                return response;
            }
        }
    }
}
