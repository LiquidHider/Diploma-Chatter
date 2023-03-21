using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<ChatMessage> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(ChatMessage item, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(UpdateChatMessage item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(ChatMessage item, CancellationToken cancellationToken);

        Task<IList<ChatMessage>> ListAsync(ChatMessageListParameters listParameters, CancellationToken cancellationToken);
    }
}
