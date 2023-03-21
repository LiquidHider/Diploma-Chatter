using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<ChatMessageModel> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(ChatMessageModel item, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(UpdateChatMessageModel item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(ChatMessageModel item, CancellationToken cancellationToken);

        Task<IList<ChatMessageModel>> ListAsync(ChatMessageListParameters listParameters, CancellationToken cancellationToken);
    }
}
