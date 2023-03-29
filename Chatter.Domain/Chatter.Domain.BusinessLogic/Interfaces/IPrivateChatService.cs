using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Abstract;
using Chatter.Domain.BusinessLogic.Models.ChatMessages;
using Chatter.Domain.BusinessLogic.Models.Chats;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;

namespace Chatter.Domain.BusinessLogic.Interfaces
{
    public interface IPrivateChatService
    {
        Task<ValueServiceResult<Guid>> SendMessageAsync(CreateChatMessage createModel, CancellationToken token);

        Task<ValueServiceResult<PrivateChat>> CreateNewChat(User participant1, User participant2, CancellationToken token);

        Task<ValueServiceResult<List<PrivateChatMessage>>> LoadChat(PrivateChat chat, CancellationToken token);

        Task<ValueServiceResult<List<PrivateChat>>> LoadPrivateChats(Guid userId, CancellationToken token);

        Task<ValueServiceResult<Guid>> MarkMessageAsRead(Guid messageId, CancellationToken token);

        Task<ValueServiceResult<Guid>> EditMessage(Guid messageId, UpdateMessage updateModel, CancellationToken token);

        Task<ValueServiceResult<Guid>> DeleteMessage(Guid messageId, CancellationToken token);
    }
}
