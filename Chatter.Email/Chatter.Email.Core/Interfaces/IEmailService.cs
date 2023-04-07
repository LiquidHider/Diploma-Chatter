using Chatter.Email.Common.ServiceResults;
using Chatter.Email.Core.Models;

namespace Chatter.Email.Core.Interfaces
{
    public interface IEmailService
    {
        ServiceResult SendEmail(EmailMessageModel model);
    }
}
