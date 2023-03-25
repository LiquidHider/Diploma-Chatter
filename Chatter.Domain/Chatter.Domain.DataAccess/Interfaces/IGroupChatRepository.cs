using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IGroupChatRepository
    {
        Task<GroupChatModel> GetGroupChatAsync(Guid id, CancellationToken cancellationToken);

        Task CreateGroupChatAsync(GroupChatModel item, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(UpdateGroupChatModel item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteGroupChatAsync(Guid id, CancellationToken cancellationToken);

        Task<IList<GroupChatModel>> ListGroupChatsAsync(GroupChatListParameters listParameters, CancellationToken cancellationToken);

        Task AddGroupParticipantAsync(GroupUserModel groupUser, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteGroupParticipantAsync(Guid participantId, CancellationToken cancellationToken);

        Task<List<GroupUserModel>> ListGroupParticipantsAsync(GroupParticipantsListParameters listParameters, CancellationToken cancellationToken);

        Task SetGroupUserAsBlockedAsync(GroupChatBlockModel blockModel, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteGroupUserFromBlockedUsersAsync(Guid blockId, CancellationToken cancellationToken);

        Task<List<GroupChatBlockModel>> ListBlockedUsers(Guid groupId, CancellationToken cancellationToken);
    }
}
