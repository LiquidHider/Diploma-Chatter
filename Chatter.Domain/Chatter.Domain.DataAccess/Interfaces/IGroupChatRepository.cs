using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.DataAccess.Interfaces
{
    internal interface IGroupChatRepository
    {
        Task<GroupChat> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(GroupChat item, CancellationToken cancellationToken);

        Task UpdateAsync(GroupChat item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(GroupChat item, CancellationToken cancellationToken);

        Task<IList<GroupChat>> ListAsync(CancellationToken cancellationToken);
    }
}
