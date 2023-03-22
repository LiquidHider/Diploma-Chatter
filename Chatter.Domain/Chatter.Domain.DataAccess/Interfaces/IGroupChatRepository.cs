using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IGroupChatRepository
    {
        Task<GroupChatModel> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(GroupChatModel item, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(UpdateGroupChatModel item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<IList<GroupChatModel>> ListAsync(GroupChatListParameters listParameters, CancellationToken cancellationToken);
    }
}
