using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IChatUserRepository
    {
        Task<ChatUser> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(ChatUser item, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(ChatUser item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(ChatUser item, CancellationToken cancellationToken);

        Task<IList<ChatUser>> ListAsync(ChatUserListParameters listParameters, CancellationToken cancellationToken);
    }
}
