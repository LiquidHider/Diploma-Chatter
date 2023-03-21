using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IGroupChatRepository
    {
        Task<GroupChat> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(GroupChat item, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(UpdateGroupChat item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(GroupChat item, CancellationToken cancellationToken);

        Task<IList<GroupChat>> ListAsync(GroupChatListParameters listParameters, CancellationToken cancellationToken);
    }
}
