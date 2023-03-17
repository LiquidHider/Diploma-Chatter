using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.DataAccess.Interfaces
{
    internal interface IChatUserRepository
    {
        Task<ChatUser> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(ChatUser item, CancellationToken cancellationToken);

        Task UpdateAsync(ChatUser item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(ChatUser item, CancellationToken cancellationToken);

        Task<IList<ChatUser>> GetAllASync(CancellationToken cancellationToken);
    }
}
