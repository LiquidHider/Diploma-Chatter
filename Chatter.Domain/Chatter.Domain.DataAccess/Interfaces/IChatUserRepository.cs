using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IChatUserRepository
    {
        Task<ChatUserModel> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(ChatUserModel item, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(UpdateChatUserModel item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<PaginatedResult<ChatUserModel, ChatUserSort>> ListAsync(ChatUserListParameters listParameters, CancellationToken cancellationToken);
    }
}
