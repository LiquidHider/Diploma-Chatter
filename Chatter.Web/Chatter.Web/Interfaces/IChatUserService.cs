using Chatter.Web.Models;

namespace Chatter.Web.Interfaces
{
    public interface IChatUserService
    {
        Task<HttpResponseMessage> GetChatUser(Guid id, string authToken);

        Task<HttpResponseMessage> GetAllUsersList(int pageNumber, int pageSize, string authToken);

        Task<HttpResponseMessage> GetChatUserContacts(Guid id, string authToken);
    }
}
