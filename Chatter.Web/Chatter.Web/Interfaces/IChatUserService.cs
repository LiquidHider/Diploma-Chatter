using Chatter.Web.Models;

namespace Chatter.Web.Interfaces
{
    public interface IChatUserService
    {
        Task<HttpResponseMessage> GetChatUser(Guid id, string authToken);

        Task<HttpResponseMessage> GetChatUsers(IEnumerable<Guid> ids, string authToken);
    }
}
