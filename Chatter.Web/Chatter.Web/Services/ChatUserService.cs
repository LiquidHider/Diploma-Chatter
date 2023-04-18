using Chatter.Web.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

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

        public Task<HttpResponseMessage> GetChatUsers(IEnumerable<Guid> ids, string authToken)
        {
            throw new NotImplementedException();
        }
    }
}
