using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Chats;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;

namespace Chatter.Domain.BusinessLogic.Interfaces
{
    public interface IChatUserService
    {
        //Create GetMainPhoto(User user) method in PhotoRepository
        Task<ValueServiceResult<ChatUser>> GetUserAsync(Guid id, CancellationToken token);

        Task<ValueServiceResult<ChatUser>> CreateNewUserAsync(CreateChatUser createModel, CancellationToken token);

        Task<ValueServiceResult<Guid>> UpdateUserAsync(UpdateChatUser updateModel, CancellationToken token);

        Task<ValueServiceResult<Guid>> DeleteUserAsync(Guid id, CancellationToken token);

        Task<ValueServiceResult<Guid>> BlockUserAsync(BlockUser blockModel, CancellationToken token);

        Task<ValueServiceResult<Guid>> UnblockUserAsync(Guid id, CancellationToken token);

    }
}
