using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.ChatMessages;
using Chatter.Domain.BusinessLogic.Models.Chats;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;

namespace Chatter.Domain.BusinessLogic.Interfaces
{
    public interface IPrivateChatService
    {
        Task<ValueServiceResult<PrivateChatMessage>> SendMessageAsync(CreateChatMessage createModel, CancellationToken token);

        Task<ValueServiceResult<PrivateChat>> CreateChatAsync(Guid member1ID, Guid member2ID, CancellationToken token);

        Task<ValueServiceResult<List<PrivateChatMessage>>> LoadChatAsync(PrivateChat chat, CancellationToken token);

        Task<ValueServiceResult<List<PrivateChat>>> LoadUserContactsAsync(Guid userId, CancellationToken token);

        Task<ValueServiceResult<Guid>> MarkMessageAsReadAsync(Guid messageId, CancellationToken token);

        Task<ValueServiceResult<Guid>> EditMessageAsync(UpdateMessage updateModel, CancellationToken token);

        Task<ValueServiceResult<Guid>> DeleteMessageAsync(Guid messageId, CancellationToken token);
    }
}
