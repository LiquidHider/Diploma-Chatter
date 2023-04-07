using Chatter.Security.Common;

namespace Chatter.Security.API.Interfaces
{
    public interface IEmailService
    {
        ServiceResult SendCongratulationsMessageToNewUser(string email);
    }
}
